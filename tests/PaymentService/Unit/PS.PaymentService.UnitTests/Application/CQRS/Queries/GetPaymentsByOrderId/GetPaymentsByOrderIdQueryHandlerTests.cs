using Microsoft.Extensions.Logging;
using Moq;
using PS.PaymentService.Application.CQRS.Queries.GetPaymentsByOrderId;
using PS.PaymentService.Application.Interfaces;
using PS.PaymentService.Domain.Entities;

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

        [Fact]
        public async Task Handle_ShouldReturnPayments_WhenPaymentsExistAndUserIsOwner()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var payments = new List<Payment>
            {
                new Payment(orderId, userId, 100),
                new Payment(orderId, userId, 200)
            };

            _paymentRepositoryMock
                .Setup(x => x.GetByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments);

            var query = new GetPaymentsByOrderIdQuery(orderId, userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.All(result, payment =>
            {
                Assert.Equal(userId, payment.Id);
            });
        }
    }
}

