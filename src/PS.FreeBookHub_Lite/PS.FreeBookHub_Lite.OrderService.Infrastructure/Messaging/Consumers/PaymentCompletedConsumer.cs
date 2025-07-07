using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.OrderService.Application.CQRS.Commands.MarkOrderAsPaid;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces.Redis;
using PS.FreeBookHub_Lite.OrderService.Common.Configuration;
using PS.FreeBookHub_Lite.OrderService.Common.Events;
using PS.FreeBookHub_Lite.OrderService.Common.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Messaging.Consumers
{
    public class PaymentCompletedConsumer : BackgroundService
    {
        private readonly ILogger<PaymentCompletedConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly RabbitMqConfig _config;

        public PaymentCompletedConsumer(
            ILogger<PaymentCompletedConsumer> logger,
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

            _logger.LogInformation(LoggerMessages.PaymentConsumerStarted, _config.PaymentCompletedQueue);
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

                    var paymentCompleted = JsonSerializer.Deserialize<PaymentCompletedEvent>(message);
                    if (paymentCompleted == null)
                        throw new JsonException("Deserialization returned null");

                    orderId = paymentCompleted.OrderId;

                    _logger.LogInformation(LoggerMessages.PaymentMessageReceived, paymentCompleted.OrderId, paymentCompleted.PaymentId);

                    using var scope = _scopeFactory.CreateScope();

                    var dedupService = scope.ServiceProvider.GetRequiredService<IEventDeduplicationService>();
                    var messageId = ea.BasicProperties.MessageId ?? paymentCompleted.PaymentId.ToString();
                    var deduplicationKey = $"processed:PaymentCompletedEvent:{messageId}";

                    if (await dedupService.IsDuplicateAsync(deduplicationKey, TimeSpan.FromHours(24), ct))
                    {
                        _logger.LogWarning("Duplicate PaymentCompletedEvent detected. MessageId: {MessageId}, PaymentId: {PaymentId}", messageId, paymentCompleted.PaymentId);

                        _channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new MarkOrderAsPaidCommand(paymentCompleted.OrderId), ct);

                    var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    _logger.LogInformation(LoggerMessages.PaymentMessageProcessed, paymentCompleted.OrderId, elapsedMs);

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, LoggerMessages.PaymentMessageDeserializeError,
                        Encoding.UTF8.GetString(ea.Body.ToArray()));

                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, LoggerMessages.PaymentProcessingError,
                        orderId?.ToString() ?? "unknown", ex.Message);

                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(_config.PaymentCompletedQueue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation(LoggerMessages.PaymentConsumerStopped, _config.PaymentCompletedQueue);
        }



        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory() { HostName = _config.HostName };
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
                _config.PaymentCompletedDeadLetterExchange,
                ExchangeType.Topic,
                durable: true);
        }

        private void DeclareQueue()
        {
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _config.PaymentCompletedDeadLetterExchange },
                { "x-dead-letter-routing-key", _config.PaymentCompletedDeadLetterRoutingKey }
            };

            _channel.QueueDeclare(
                queue: _config.PaymentCompletedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);
        }

        private void BindQueue()
        {
            _channel.QueueBind(
                queue: _config.PaymentCompletedQueue,
                exchange: _config.ExchangeName,
                routingKey: _config.PaymentCompletedRoutingKey);
        }

    }
}
