using Microsoft.Extensions.Logging;
using Moq;
using PS.PaymentService.Application.CQRS.Queries.GetPaymentById;
using PS.PaymentService.Application.Interfaces;
using PS.PaymentService.Domain.Entities;
using PS.PaymentService.Domain.Exceptions.Payment;

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

        [Fact]
        public async Task Handle_ShouldReturnPaymentResponse_WhenPaymentExistsAndUserIsOwner()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var payment = new Payment(paymentId, userId, 100);

            _repoMock
                .Setup(x => x.GetByIdAsync(paymentId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(payment);

            var query = new GetPaymentByIdQuery(paymentId, userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payment.Id, result.Id);
            Assert.Equal(payment.Amount, result.Amount);
        }

        [Fact]
        public async Task Handle_ShouldThrowPaymentNotFoundException_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _repoMock
                .Setup(x => x.GetByIdAsync(paymentId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((Payment?)null);

            var query = new GetPaymentByIdQuery(paymentId, userId);

            // Act & Assert
            await Assert.ThrowsAsync<PaymentNotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedPaymentAccessException_WhenUserIsNotOwner()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var payment = new Payment(paymentId, ownerId, 100);

            _repoMock
                .Setup(x => x.GetByIdAsync(paymentId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(payment);

            var query = new GetPaymentByIdQuery(paymentId, anotherUserId);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedPaymentAccessException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var payment = new Payment(paymentId, userId, 100);

            _repoMock
                .Setup(x => x.GetByIdAsync(paymentId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(payment);

            var query = new GetPaymentByIdQuery(paymentId, userId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repoMock.Verify(x =>
                x.GetByIdAsync(paymentId, It.IsAny<CancellationToken>(), true),
                Times.Once);
        }

    }
}