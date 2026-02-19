using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Application.CQRS.Queries.GetAllOrdersByUserId;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Domain.Entities;

namespace PS.OrderService.UnitTests.Application.CQRS.Queries.GetAllOrdersByUserld
{
    public class GetAllOrdersByUserIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<ILogger<GetAllOrdersByUserIdQueryHandler>> _loggerMock = new();

        private GetAllOrdersByUserIdQueryHandler CreateHandler() =>
            new(_orderRepositoryMock.Object, _loggerMock.Object);


        [Fact]
        public async Task Handle_UserHasOrders_ShouldReturnOrderResponses()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order(userId, "Address 1"),
                new Order(userId, "Address 2")
            };

            _orderRepositoryMock
                .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);

            var handler = CreateHandler();
            var query = new GetAllOrdersByUserIdQuery(userId);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            _orderRepositoryMock.Verify(r =>
                r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
