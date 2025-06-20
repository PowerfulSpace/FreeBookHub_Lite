using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Application.Interfaces;
using PS.FreeBookHub_Lite.OrderService.Common.Events;
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

        public PaymentCompletedConsumer(ILogger<PaymentCompletedConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory() { HostName = "localhost" }; // Или конфиг
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "payment.completed", durable: true, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var paymentCompleted = JsonSerializer.Deserialize<PaymentCompletedEvent>(message);
                    if (paymentCompleted is null)
                        return;

                    _logger.LogInformation("Получено событие PaymentCompleted: OrderId={OrderId}", paymentCompleted.OrderId);

                    using var scope = _scopeFactory.CreateScope();
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    var order = await orderRepository.GetByIdAsync(paymentCompleted.OrderId, stoppingToken);
                    if (order == null)
                    {
                        _logger.LogWarning("Заказ не найден: {OrderId}", paymentCompleted.OrderId);
                        return;
                    }

                    order.MarkAsPaid();
                    await orderRepository.UpdateAsync(order, stoppingToken);

                    _logger.LogInformation("Заказ помечен как оплаченный: OrderId={OrderId}", order.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке PaymentCompletedEvent");
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
        }
    }
}
