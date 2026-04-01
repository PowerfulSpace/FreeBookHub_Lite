using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PS.OrderService.Application.Interfaces.Redis;
using PS.OrderService.Common.Configuration;
using PS.OrderService.Common.Events;
using PS.OrderService.Infrastructure.Messaging.Consumers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PS.OrderService.UnitTests.Infrastructure.Messaging.Consumers
{
    public class PaymentCompletedConsumerTests
    {
        private readonly Mock<ILogger<PaymentCompletedConsumer>> _loggerMock = new();
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock = new();
        private readonly Mock<IServiceScope> _scopeMock = new();
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();

        private readonly Mock<IModel> _channelMock = new();
        private readonly Mock<IConnection> _connectionMock = new();

        private readonly Mock<IEventDeduplicationService> _dedupMock = new();
        private readonly Mock<IMediator> _mediatorMock = new();

        private PaymentCompletedConsumer CreateConsumer()
        {
            var config = Options.Create(new RabbitMqConfig());

            _connectionMock.Setup(c => c.CreateModel()).Returns(_channelMock.Object);

            _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);
            _scopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);

            _serviceProviderMock.Setup(x => x.GetService(typeof(IEventDeduplicationService)))
                                .Returns(_dedupMock.Object);

            _serviceProviderMock.Setup(x => x.GetService(typeof(IMediator)))
                                .Returns(_mediatorMock.Object);

            return new TestablePaymentCompletedConsumer(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                config,
                _connectionMock.Object,
                _channelMock.Object);
        }

        [Fact]
        public async Task Consumer_ShouldAck_WhenDuplicateDetected()
        {
            // Arrange
            var consumer = CreateConsumer();

            _dedupMock
                .Setup(x => x.IsDuplicateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var paymentEvent = new PaymentCompletedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paymentEvent));

            var ea = new BasicDeliverEventArgs
            {
                Body = body,
                DeliveryTag = 1,
                BasicProperties = new Mock<IBasicProperties>().Object
            };

            var eventingConsumer = new EventingBasicConsumer(_channelMock.Object);

            // перехватываем обработчик
            Func<object, BasicDeliverEventArgs, Task>? handler = null;
            eventingConsumer.Received += (s, e) =>
            {
                handler?.Invoke(s, e);
            };

            // Act
            await InvokeConsumer(consumer, ea);

            // Assert
            _channelMock.Verify(c => c.BasicAck(1, false), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<Unit>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // helper для вызова private логики через ExecuteAsync
        private async Task InvokeConsumer(PaymentCompletedConsumer consumer, BasicDeliverEventArgs ea)
        {
            var executeMethod = typeof(PaymentCompletedConsumer)
                .GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

            var task = (Task)executeMethod!.Invoke(consumer, new object[] { CancellationToken.None })!;

            // имитируем consumer
            var eventingConsumer = new EventingBasicConsumer(_channelMock.Object);
            await Task.Run(() =>
            {
                eventingConsumer.HandleBasicDeliver(
                    consumerTag: "",
                    deliveryTag: ea.DeliveryTag,
                    redelivered: false,
                    exchange: "",
                    routingKey: "",
                    properties: ea.BasicProperties,
                    body: ea.Body);
            });
        }
    }

    internal class TestablePaymentCompletedConsumer : PaymentCompletedConsumer
    {
        public TestablePaymentCompletedConsumer(
            ILogger<PaymentCompletedConsumer> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqConfig> config,
            IConnection connection,
            IModel channel)
            : base(logger, scopeFactory, config)
        {
            typeof(PaymentCompletedConsumer)
                .GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(this, connection);

            typeof(PaymentCompletedConsumer)
                .GetField("_channel", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(this, channel);
        }
    }
}

