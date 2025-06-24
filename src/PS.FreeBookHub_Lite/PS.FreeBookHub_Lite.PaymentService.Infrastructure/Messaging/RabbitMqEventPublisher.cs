using PS.FreeBookHub_Lite.PaymentService.Common.Events.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.PaymentService.Common.Logging;

namespace PS.FreeBookHub_Lite.PaymentService.Infrastructure.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string ExchangeName = "bookhub.exchange";

        public RabbitMqEventPublisher(ILogger<RabbitMqEventPublisher> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);

            _logger.LogInformation(LoggerMessages.EventPublisherCreated, ExchangeName);
        }

        public Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation(LoggerMessages.EventPublished, typeof(TEvent).Name, routingKey);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.EventPublishError,
                    typeof(TEvent).Name, ex.Message);
                throw;
            }

        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
