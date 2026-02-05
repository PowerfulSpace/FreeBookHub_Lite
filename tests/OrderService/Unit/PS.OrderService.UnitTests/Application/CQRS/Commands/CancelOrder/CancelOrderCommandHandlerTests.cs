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

    }
}
