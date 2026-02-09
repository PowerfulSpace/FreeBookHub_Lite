using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Application.CQRS.Commands.MarkOrderAsPaid;
using PS.OrderService.Application.Interfaces;
using PS.OrderService.Domain.Entities;

namespace PS.OrderService.UnitTests.Application.CQRS.Commands.MarkOrderAsPaid
{
    public class MarkOrderAsPaidCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<ILogger<MarkOrderAsPaidCommandHandler>> _loggerMock = new();
        private readonly MarkOrderAsPaidCommandHandler _handler;


        public MarkOrderAsPaidCommandHandlerTests()
        {
            _handler = new MarkOrderAsPaidCommandHandler(
                _orderRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldMarkOrderAsPaid_WhenOrderExists()
        {
            var orderId = Guid.NewGuid();
            var orderMock = new Mock<Order>();

            _orderRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    orderId,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(orderMock.Object);

            var command = new MarkOrderAsPaidCommand(orderId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(Unit.Value, result);

            orderMock.Verify(o => o.MarkAsPaid(), Times.Once);

            _orderRepositoryMock.Verify(r =>
                r.UpdateAsync(orderMock.Object, It.IsAny<CancellationToken>()),
                Times.Once);
        }

    }
}
