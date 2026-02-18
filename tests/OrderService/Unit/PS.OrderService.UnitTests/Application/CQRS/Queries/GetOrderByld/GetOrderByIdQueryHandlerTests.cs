using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Application.CQRS.Queries.GetOrderById;
using PS.OrderService.Application.DTOs;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Domain.Entities;
using PS.OrderService.Domain.Exceptions.Order;

namespace PS.OrderService.UnitTests.Application.CQRS.Queries.GetOrderByld
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<ILogger<GetOrderByIdQueryHandler>> _loggerMock = new();

        private GetOrderByIdQueryHandler CreateHandler() =>
            new(_orderRepositoryMock.Object, _loggerMock.Object);

        [Fact]
        public async Task Handle_OrderExists_ShouldReturnOrderResponse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), "Berlin, Test street");

            _orderRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    orderId,
                    It.IsAny<CancellationToken>(),
                    true))
                .ReturnsAsync(order);

            var handler = CreateHandler();
            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OrderResponse>(result);

            _orderRepositoryMock.Verify(r =>
                r.GetByIdAsync(orderId, It.IsAny<CancellationToken>(), true),
                Times.Once);
        }

        [Fact]
        public async Task Handle_OrderNotFound_ShouldThrowOrderNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _orderRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    orderId,
                    It.IsAny<CancellationToken>(),
                    true))
                .ReturnsAsync((Order?)null);

            var handler = CreateHandler();
            var query = new GetOrderByIdQuery(orderId);

            // Act + Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(() =>
                handler.Handle(query, default));

            _orderRepositoryMock.Verify(r =>
                r.GetByIdAsync(orderId, It.IsAny<CancellationToken>(), true),
                Times.Once);
        }
    }
}

