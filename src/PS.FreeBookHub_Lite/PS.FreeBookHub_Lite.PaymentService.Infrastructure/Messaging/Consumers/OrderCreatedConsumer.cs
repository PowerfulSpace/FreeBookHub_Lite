using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
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

        private const string ExchangeName = "bookhub.exchange";

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory() { HostName = "localhost" }; // Или из конфига
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);

            _channel.QueueDeclare(queue: "order.created", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: "order.created", exchange: "bookhub.exchange", routingKey: "order.created");

            _logger.LogInformation(LoggerMessages.OrderConsumerStarted, "order.created");
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
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentBookService>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                    _logger.LogInformation(LoggerMessages.PaymentProcessingStarted, orderCreated.OrderId);

                    var paymentResponse = await paymentService.ProcessPaymentAsync(new CreatePaymentRequest
                    {
                        OrderId = orderCreated.OrderId,
                        UserId = orderCreated.UserId,
                        Amount = orderCreated.Amount
                    }, stoppingToken);

                    _logger.LogInformation(LoggerMessages.PaymentProcessed, paymentResponse.Id, paymentResponse.OrderId);


                    var paymentCompleted = new PaymentCompletedEvent(
                        OrderId: paymentResponse.OrderId,
                        PaymentId: paymentResponse.Id,
                        CompletedAt: DateTime.UtcNow
                    );

                    await publisher.PublishAsync(paymentCompleted, "payment.completed", stoppingToken);

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

            _channel.BasicConsume(queue: "order.created", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation(LoggerMessages.OrderConsumerStopped, "order.created");
        }
    }
}
