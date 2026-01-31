using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PS.OrderService.Application.CQRS.Commands.CreateOrder;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Common.Configuration;
using PS.OrderService.Common.Events.Interfaces;

namespace PS.OrderService.UnitTests.Application.CQRS.Commands.CreateOrder
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<IEventPublisher> _eventPublisherMock = new();
        private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock = new();
        private readonly Mock<IOptions<RabbitMqConfig>> _optionsMock = new();

        private CreateOrderCommandHandler CreateHandler()
        {
            _optionsMock.Setup(o => o.Value)
                .Returns(new RabbitMqConfig
                {
                    RoutingKeys = new RabbitMqConfig.RoutingKeysConfig
                    {
                        OrderCreatedRoutingKey = "order.created"
                    }
                });

            return new CreateOrderCommandHandler(
                _orderRepositoryMock.Object,
                _loggerMock.Object,
                _eventPublisherMock.Object,
                _optionsMock.Object);
        }

    }
}
