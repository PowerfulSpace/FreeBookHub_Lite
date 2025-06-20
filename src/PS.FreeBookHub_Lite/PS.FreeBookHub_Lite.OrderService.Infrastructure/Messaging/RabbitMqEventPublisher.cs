using PS.FreeBookHub_Lite.OrderService.Common.Events.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string ExchangeName = "bookhub.exchange";

        public RabbitMqEventPublisher()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost", // можно вынести в конфиг
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
        }

        public Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default)
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

            return Task.CompletedTask;
        }
    }
}