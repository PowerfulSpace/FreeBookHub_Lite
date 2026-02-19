using Microsoft.Extensions.Logging;
using Moq;
using PS.OrderService.Application.CQRS.Queries.GetAllOrdersByUserId;
using PS.OrderService.Application.Interfaces;

namespace PS.OrderService.UnitTests.Application.CQRS.Queries.GetAllOrdersByUserld
{
    public class GetAllOrdersByUserIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<ILogger<GetAllOrdersByUserIdQueryHandler>> _loggerMock = new();

        private GetAllOrdersByUserIdQueryHandler CreateHandler() =>
            new(_orderRepositoryMock.Object, _loggerMock.Object);
    }
}
