using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PS.OrderService.Common.Configuration;
using PS.OrderService.Infrastructure.Messaging;
using RabbitMQ.Client;
using System.Reflection;

namespace PS.OrderService.UnitTests.Infrastructure.Messaging
{
    public class RabbitMqEventPublisherTests
    {
        private readonly Mock<ILogger<RabbitMqEventPublisher>> _loggerMock = new();
        private readonly Mock<IModel> _channelMock = new();
        private readonly Mock<IConnection> _connectionMock = new();

        private RabbitMqEventPublisher CreatePublisher()
        {
            var config = Options.Create(new RabbitMqConfig
            {
                HostName = "localhost",
                ExchangeName = "test.exchange"
            });

            _connectionMock
                .Setup(c => c.CreateModel())
                .Returns(_channelMock.Object);

            return new TestableRabbitMqEventPublisher(
                _loggerMock.Object,
                config,
                _connectionMock.Object,
                _channelMock.Object);
        }

    }


    internal class TestableRabbitMqEventPublisher : RabbitMqEventPublisher
    {
        public TestableRabbitMqEventPublisher(
            ILogger<RabbitMqEventPublisher> logger,
            IOptions<RabbitMqConfig> config,
            IConnection connection,
            IModel channel)
            : base(logger, config)
        {
            typeof(RabbitMqEventPublisher)
                .GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(this, connection);

            typeof(RabbitMqEventPublisher)
                .GetField("_channel", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(this, channel);
        }
    }
}
