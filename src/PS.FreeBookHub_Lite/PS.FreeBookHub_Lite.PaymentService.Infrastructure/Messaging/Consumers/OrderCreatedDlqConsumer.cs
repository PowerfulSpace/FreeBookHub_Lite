using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.PaymentService.Common.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging.Consumers
{
    public class OrderCreatedDlqConsumer : BackgroundService
    {
        private readonly ILogger<OrderCreatedDlqConsumer> _logger;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly RabbitMqConfig _config;

        public OrderCreatedDlqConsumer(
            ILogger<OrderCreatedDlqConsumer> logger,
            IOptions<RabbitMqConfig> config)
        {
            _logger = logger;
            _config = config.Value;

            var factory = new ConnectionFactory() { HostName = _config.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                _config.OrderCreatedDeadLetterExchange,
                ExchangeType.Topic,
                durable: true);

            _channel.QueueDeclare(
                queue: _config.OrderCreatedDeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.QueueBind(
                queue: _config.OrderCreatedDeadLetterQueue,
                exchange: _config.OrderCreatedDeadLetterExchange,
                routingKey: _config.OrderCreatedDeadLetterRoutingKey);

            _logger.LogInformation("DLQ Consumer initialized for OrderCreated queue: {Queue}", _config.OrderCreatedDeadLetterQueue);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogWarning("Received message from OrderCreated DLQ: {Message}", message);

                // Пока просто логируем и подтверждаем
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(
                queue: _config.OrderCreatedDeadLetterQueue,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation("DLQ Consumer stopped for queue: {Queue}", _config.OrderCreatedDeadLetterQueue);
        }
    }
}
