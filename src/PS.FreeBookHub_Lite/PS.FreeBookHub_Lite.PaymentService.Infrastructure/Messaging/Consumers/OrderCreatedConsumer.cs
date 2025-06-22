using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.PaymentService.Application.DTOs;
using PS.FreeBookHub_Lite.PaymentService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.PaymentService.Common.Events;
using PS.FreeBookHub_Lite.PaymentService.Common.Events.Interfaces;
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

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory() { HostName = "localhost" }; // Или из конфига
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "order.created", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: "order.created", exchange: "bookhub.exchange", routingKey: "order.created");
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

                    var orderCreated = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                    if (orderCreated == null)
                        return;

                    _logger.LogInformation("Получено событие OrderCreated: OrderId={OrderId}", orderCreated.OrderId);

                    using var scope = _scopeFactory.CreateScope();
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentBookService>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                    var paymentResponse = await paymentService.ProcessPaymentAsync(new CreatePaymentRequest
                    {
                        OrderId = orderCreated.OrderId,
                        UserId = orderCreated.UserId,
                        Amount = orderCreated.Amount
                    }, stoppingToken);

                    var paymentCompleted = new PaymentCompletedEvent(
                        OrderId: paymentResponse.OrderId,
                        PaymentId: paymentResponse.Id,
                        CompletedAt: DateTime.UtcNow
                    );

                    await publisher.PublishAsync(paymentCompleted, "payment.completed", stoppingToken);

                    _logger.LogInformation("Платеж обработан и событие отправлено: PaymentId={PaymentId}", paymentResponse.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке OrderCreatedEvent");
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
        }
    }
}
