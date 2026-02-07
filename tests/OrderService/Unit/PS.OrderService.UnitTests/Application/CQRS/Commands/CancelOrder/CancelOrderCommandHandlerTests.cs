using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Application.CQRS.Commands.CancelOrder;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Domain.Entities;

namespace PS.OrderService.UnitTests.Application.CQRS.Commands.CancelOrder
{
    public class CancelOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ILogger<CancelOrderCommandHandler>> _loggerMock;
        private readonly CancelOrderCommandHandler _handler;

        public CancelOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _loggerMock = new Mock<ILogger<CancelOrderCommandHandler>>();

            _handler = new CancelOrderCommandHandler(
                _orderRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCancelOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Mock<Order>();

            _orderRepositoryMock
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(order.Object);

            var command = new CancelOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);

            order.Verify(o => o.Cancel(), Times.Once);

            _orderRepositoryMock.Verify(r =>
                r.UpdateAsync(order.Object, It.IsAny<CancellationToken>()),
                Times.Once);
        }

    }
}
