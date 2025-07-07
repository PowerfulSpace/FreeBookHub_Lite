using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Commands.ProcessPayment;
using PS.FreeBookHub_Lite.PaymentService.Application.Interfaces.Redis;
using PS.FreeBookHub_Lite.PaymentService.Common.Configuration;
using PS.FreeBookHub_Lite.PaymentService.Common.Events;
using PS.FreeBookHub_Lite.PaymentService.Common.Events.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Common.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly RabbitMqConfig _config;


        public OrderCreatedConsumer(
            ILogger<OrderCreatedConsumer> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqConfig> config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _config = config.Value;

            _connection = CreateConnection();
            _channel = CreateChannel(_connection);
            DeclareExchanges();
            DeclareQueue();
            BindQueue();

            _logger.LogInformation(LoggerMessages.OrderConsumerStarted, _config.OrderCreatedQueue);
        }

        protected override Task ExecuteAsync(CancellationToken ct)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var startTime = DateTime.UtcNow;
                Guid? orderId = null;

                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var orderCreated = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                    if (orderCreated == null)
                        throw new JsonException("Deserialization returned null");

                    orderId = orderCreated.OrderId;
                    _logger.LogInformation(LoggerMessages.OrderMessageReceived, orderCreated.OrderId, orderCreated.UserId, orderCreated.Amount);


                    _logger.LogInformation(LoggerMessages.OrderProcessingStarted, orderCreated.OrderId);

                    using var scope = _scopeFactory.CreateScope();

                    var dedupService = scope.ServiceProvider.GetRequiredService<IEventDeduplicationService>();
                    var messageId = ea.BasicProperties.MessageId ?? orderCreated.OrderId.ToString();
                    var deduplicationKey = $"processed:OrderCreatedEvent:{messageId}";

                    if (await dedupService.IsDuplicateAsync(deduplicationKey, TimeSpan.FromHours(24), ct))
                    {
                        _logger.LogWarning("Duplicate OrderCreatedEvent detected. MessageId: {MessageId}, OrderId: {OrderId}", messageId, orderCreated.OrderId);
                           
                        _channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                    _logger.LogInformation(LoggerMessages.PaymentProcessingStarted, orderCreated.OrderId);

                    var command = new ProcessPaymentCommand(
                        orderCreated.OrderId,
                        orderCreated.UserId,
                        orderCreated.Amount,
                        ""
                    );

                    var paymentResponse = await mediator.Send(command, ct);

                    _logger.LogInformation(LoggerMessages.PaymentProcessed, paymentResponse.Id, paymentResponse.OrderId);


                    var paymentCompleted = new PaymentCompletedEvent(
                        OrderId: paymentResponse.OrderId,
                        PaymentId: paymentResponse.Id,
                        CompletedAt: DateTime.UtcNow
                    );

                    await publisher.PublishAsync(paymentCompleted, _config.PaymentCompletedRoutingKey, ct);

                    _logger.LogInformation(LoggerMessages.PaymentEventPublished, paymentResponse.OrderId, paymentResponse.Id);

                    var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    _logger.LogInformation(LoggerMessages.OrderMessageProcessed,
                        orderCreated.OrderId, elapsedMs);

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, LoggerMessages.OrderMessageDeserializeError,
                        Encoding.UTF8.GetString(ea.Body.ToArray()));

                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, LoggerMessages.OrderProcessingError,
                        orderId?.ToString() ?? "unknown", ex.Message);

                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: _config.OrderCreatedQueue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation(LoggerMessages.OrderConsumerStopped, _config.OrderCreatedQueue);
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName
            };
            return factory.CreateConnection();
        }

        private IModel CreateChannel(IConnection connection)
        {
            return connection.CreateModel();
        }

        private void DeclareExchanges()
        {
            _channel.ExchangeDeclare(
                _config.ExchangeName,
                ExchangeType.Topic,
                durable: true);

            _channel.ExchangeDeclare(
               _config.OrderCreatedDeadLetterExchange,
               ExchangeType.Topic,
               durable: true);
        }

        private void DeclareQueue()
        {
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _config.OrderCreatedDeadLetterExchange },
                { "x-dead-letter-routing-key", _config.OrderCreatedDeadLetterRoutingKey }
            };

            _channel.QueueDeclare(
                _config.OrderCreatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);
        }

        private void BindQueue()
        {
            _channel.QueueBind(
                queue: _config.OrderCreatedQueue,
                exchange: _config.ExchangeName,
                routingKey: _config.OrderCreatedRoutingKey);
        }
    }
}
