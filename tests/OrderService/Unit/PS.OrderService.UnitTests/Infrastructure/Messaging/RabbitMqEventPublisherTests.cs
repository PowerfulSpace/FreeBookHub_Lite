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

        [Fact]
        public async Task PublishAsync_ShouldPublishMessage()
        {
            var publisher = CreatePublisher();

            var testEvent = new
            {
                OrderId = Guid.NewGuid(),
                Amount = 100
            };

            await publisher.PublishAsync(testEvent, "order.created");

            _channelMock.Verify(c => c.BasicPublish(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<byte[]>()),
                Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
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
