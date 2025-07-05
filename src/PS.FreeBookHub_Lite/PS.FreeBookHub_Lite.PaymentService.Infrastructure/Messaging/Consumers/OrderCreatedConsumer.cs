using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.PaymentService.Application.CQRS.Commands.ProcessPayment;
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

            var factory = new ConnectionFactory() { HostName = _config.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _config.ExchangeName, ExchangeType.Topic, durable: true);

            _channel.QueueDeclare(_config.OrderCreatedQueue, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: _config.OrderCreatedQueue, exchange: _config.ExchangeName, routingKey: _config.OrderCreatedRoutingKey);

            _logger.LogInformation(LoggerMessages.OrderConsumerStarted, _config.OrderCreatedQueue);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
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
                    {
                        _logger.LogWarning(LoggerMessages.OrderMessageDeserializeError, message);
                        return;
                    }

                    orderId = orderCreated.OrderId;
                    _logger.LogInformation(LoggerMessages.OrderMessageReceived, orderCreated.OrderId, orderCreated.UserId, orderCreated.Amount);


                    _logger.LogInformation(LoggerMessages.OrderProcessingStarted, orderCreated.OrderId);

                    using var scope = _scopeFactory.CreateScope();

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                    _logger.LogInformation(LoggerMessages.PaymentProcessingStarted, orderCreated.OrderId);

                    var command = new ProcessPaymentCommand(
                        orderCreated.OrderId,
                        orderCreated.UserId,
                        orderCreated.Amount,
                        ""
                    );

                    var paymentResponse = await mediator.Send(command, stoppingToken);

                    _logger.LogInformation(LoggerMessages.PaymentProcessed, paymentResponse.Id, paymentResponse.OrderId);


                    var paymentCompleted = new PaymentCompletedEvent(
                        OrderId: paymentResponse.OrderId,
                        PaymentId: paymentResponse.Id,
                        CompletedAt: DateTime.UtcNow
                    );

                    await publisher.PublishAsync(paymentCompleted, _config.PaymentCompletedRoutingKey, stoppingToken);

                    _logger.LogInformation(LoggerMessages.PaymentEventPublished, paymentResponse.OrderId, paymentResponse.Id);

                    var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    _logger.LogInformation(LoggerMessages.OrderMessageProcessed,
                        orderCreated.OrderId, elapsedMs);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, LoggerMessages.OrderProcessingError,
                        orderId?.ToString() ?? "unknown", ex.Message);
                }
            };

            _channel.BasicConsume(queue: _config.OrderCreatedQueue, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation(LoggerMessages.OrderConsumerStopped, _config.OrderCreatedQueue);
        }
    }
}
