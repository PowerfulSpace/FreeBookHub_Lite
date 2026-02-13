using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Application.CQRS.Queries.GetOrderById;
using PS.OrderService.Application.Interfaces;

namespace PS.OrderService.UnitTests.Application.CQRS.Queries.GetOrderByld
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<ILogger<GetOrderByIdQueryHandler>> _loggerMock = new();

        private GetOrderByIdQueryHandler CreateHandler() =>
            new(_orderRepositoryMock.Object, _loggerMock.Object);
    }
}
