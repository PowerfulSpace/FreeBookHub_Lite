using Microsoft.Extensions.Logging;
using Moq;
using PS.PaymentService.Application.CQRS.Queries.GetPaymentById;
using PS.PaymentService.Application.Interfaces;

namespace PS.PaymentService.UnitTests.Application.CQRS.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandlerTests
    {
        private readonly Mock<IPaymentRepository> _repoMock;
        private readonly Mock<ILogger<GetPaymentByIdQueryHandler>> _loggerMock;
        private readonly GetPaymentByIdQueryHandler _handler;

        public GetPaymentByIdQueryHandlerTests()
        {
            _repoMock = new Mock<IPaymentRepository>();
            _loggerMock = new Mock<ILogger<GetPaymentByIdQueryHandler>>();

            _handler = new GetPaymentByIdQueryHandler(
                _repoMock.Object,
                _loggerMock.Object);
        }

    }
}
