using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
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

        private const string ExchangeName = "bookhub.exchange";

        public PaymentCompletedConsumer(ILogger<PaymentCompletedConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory() { HostName = "localhost" }; // Или конфиг
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);

            _channel.QueueDeclare(queue: "payment.completed", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("payment.completed", "bookhub.exchange", "payment.completed");

            _logger.LogInformation(LoggerMessages.PaymentConsumerStarted, "payment.completed");
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

                    var paymentCompleted = JsonSerializer.Deserialize<PaymentCompletedEvent>(message);
                    if (paymentCompleted is null)
                    {
                        _logger.LogWarning(LoggerMessages.PaymentMessageDeserializeError, message);
                        return;
                    }

                    orderId = paymentCompleted.OrderId;
                    _logger.LogInformation(LoggerMessages.PaymentMessageReceived, paymentCompleted.OrderId, paymentCompleted.PaymentId);


                    using var scope = _scopeFactory.CreateScope();
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    _logger.LogInformation(LoggerMessages.PaymentMessageProcessing, paymentCompleted.OrderId);

                    var order = await orderRepository.GetByIdAsync(paymentCompleted.OrderId, stoppingToken);
                    if (order == null)
                    {
                        _logger.LogWarning(LoggerMessages.PaymentOrderNotFound, paymentCompleted.OrderId);
                        return;
                    }

                    order.MarkAsPaid();
                    await orderRepository.UpdateAsync(order, stoppingToken);

                    _logger.LogInformation(LoggerMessages.PaymentOrderMarkedAsPaid, order.Id);

                    var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    _logger.LogInformation(LoggerMessages.PaymentMessageProcessed, paymentCompleted.OrderId, elapsedMs);

                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, LoggerMessages.PaymentMessageDeserializeError,
                        Encoding.UTF8.GetString(ea.Body.ToArray()));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, LoggerMessages.PaymentProcessingError,
                        orderId?.ToString() ?? "unknown", ex.Message);
                }
            };

            _channel.BasicConsume(queue: "payment.completed", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation(LoggerMessages.PaymentConsumerStopped, "payment.completed");
        }
    }
}
