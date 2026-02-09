using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Application.CQRS.Commands.MarkOrderAsPaid;
using PS.OrderService.Application.Interfaces;

namespace PS.OrderService.UnitTests.Application.CQRS.Commands.MarkOrderAsPaid
{
    public class MarkOrderAsPaidCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<ILogger<MarkOrderAsPaidCommandHandler>> _loggerMock = new();
        private readonly MarkOrderAsPaidCommandHandler _handler;


    }
}
