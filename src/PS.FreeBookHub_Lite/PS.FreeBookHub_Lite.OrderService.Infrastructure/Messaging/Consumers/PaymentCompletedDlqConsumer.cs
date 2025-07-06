using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.OrderService.Common.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Messaging.Consumers
{
    public class PaymentCompletedDlqConsumer : BackgroundService
    {
        private readonly ILogger<PaymentCompletedDlqConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly RabbitMqConfig _config;

        public PaymentCompletedDlqConsumer(
           ILogger<PaymentCompletedDlqConsumer> logger,
           IServiceScopeFactory scopeFactory,
           IOptions<RabbitMqConfig> config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _config = config.Value;

            _connection = CreateConnection();
            _channel = CreateChannel(_connection);
            DeclareExchange();
            DeclareQueue();
            BindQueue();

            _logger.LogInformation("DLQ Consumer initialized for queue: {Queue}", _config.PaymentCompletedDeadLetterQueue);
        }



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogWarning("Received message from DLQ: {Message}", message);

                // Здесь можно:
                // - сохранить в базу
                // - отправить уведомление
                // - повторно опубликовать куда-то
                // Пока просто логируем и подтверждаем.

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(
                queue: _config.PaymentCompletedDeadLetterQueue,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
            _logger.LogInformation("DLQ Consumer stopped for queue: {Queue}", _config.PaymentCompletedDeadLetterQueue);
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.HostName
            };
            return factory.CreateConnection();
        }

        private IModel CreateChannel(IConnection connection)
        {
            return connection.CreateModel();
        }

        private void DeclareExchange()
        {
            _channel.ExchangeDeclare(
                _config.PaymentCompletedDeadLetterExchange,
                ExchangeType.Topic,
                durable: true);
        }

        private void DeclareQueue()
        {
            _channel.QueueDeclare(
                queue: _config.PaymentCompletedDeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }

        private void BindQueue()
        {
            _channel.QueueBind(
                queue: _config.PaymentCompletedDeadLetterQueue,
                exchange: _config.PaymentCompletedDeadLetterExchange,
                routingKey: _config.PaymentCompletedDeadLetterRoutingKey);
        }
    }
}
