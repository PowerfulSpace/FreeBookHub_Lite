using PS.FreeBookHub_Lite.OrderService.Common.Events.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Common.Logging;
using Microsoft.Extensions.Options;
using PS.FreeBookHub_Lite.OrderService.Common.Configuration;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMqConfig _config;

        public RabbitMqEventPublisher(ILogger<RabbitMqEventPublisher> logger, IOptions<RabbitMqConfig> config)
        {
            _logger = logger;
            _config = config.Value;

            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_config.ExchangeName, ExchangeType.Topic, durable: true);

            _logger.LogInformation(LoggerMessages.EventPublisherCreated, _config.ExchangeName);
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
                    exchange: _config.ExchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation(LoggerMessages.OrderEventPublished,
                   typeof(TEvent).Name, routingKey);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.OrderEventPublishError,
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