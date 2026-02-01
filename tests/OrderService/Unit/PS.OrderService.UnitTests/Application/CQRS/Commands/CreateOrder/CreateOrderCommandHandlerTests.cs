using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PS.OrderService.Application.CQRS.Commands.CreateOrder;
using PS.OrderService.Application.DTOs;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Common.Configuration;
using PS.OrderService.Common.Events;
using PS.OrderService.Common.Events.Interfaces;
using PS.OrderService.Domain.Entities;

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

        [Fact]
        public async Task Handle_ShouldCreateOrder_SaveItAndPublishEvent()
        {
            var userId = Guid.NewGuid();

            var command = new CreateOrderCommand
            {
                UserId = userId,
                ShippingAddress = "Some address",
                Items =
                {
                    new CreateOrderItemRequest
                    {
                        BookId = Guid.NewGuid(),
                        Quantity = 2,
                        UnitPrice = 10m
                    },
                    new CreateOrderItemRequest
                    {
                        BookId = Guid.NewGuid(),
                        Quantity = 1,
                        UnitPrice = 20m
                    }
                }
            };

            Order? savedOrder = null;

            _orderRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Callback<Order, CancellationToken>((order, _) => savedOrder = order)
                .Returns(Task.CompletedTask);

            _eventPublisherMock
                .Setup(p => p.PublishAsync(
                    It.IsAny<OrderCreatedEvent>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            var result = await handler.Handle(command, default);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(40m, result.TotalPrice);

            Assert.NotNull(savedOrder);
            Assert.Equal(userId, savedOrder!.UserId);
            Assert.Equal(40m, savedOrder.TotalPrice);

            _orderRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _eventPublisherMock.Verify(
                p => p.PublishAsync(
                    It.IsAny<OrderCreatedEvent>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


    }
}
