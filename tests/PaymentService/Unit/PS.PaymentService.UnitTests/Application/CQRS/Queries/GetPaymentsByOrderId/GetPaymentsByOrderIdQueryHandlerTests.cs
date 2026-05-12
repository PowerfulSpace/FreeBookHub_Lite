using Microsoft.Extensions.Logging;
using Moq;
using PS.PaymentService.Application.CQRS.Queries.GetPaymentsByOrderId;
using PS.PaymentService.Application.Interfaces;

namespace PS.PaymentService.UnitTests.Application.CQRS.Queries.GetPaymentsByOrderId
{
    public class GetPaymentsByOrderIdQueryHandlerTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<ILogger<GetPaymentsByOrderIdQueryHandler>> _loggerMock;
        private readonly GetPaymentsByOrderIdQueryHandler _handler;

        public GetPaymentsByOrderIdQueryHandlerTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _loggerMock = new Mock<ILogger<GetPaymentsByOrderIdQueryHandler>>();

            _handler = new GetPaymentsByOrderIdQueryHandler(
                _paymentRepositoryMock.Object,
                _loggerMock.Object);
        }
    }
}
