using PS.PaymentService.Domain.Entities;
using PS.PaymentService.Domain.Enums;

namespace PS.PaymentService.UnitTests.Domain
{
    public class PaymentTests
    {
        [Fact]
        public void Constructor_ShouldCreatePayment_WhenDataIsValid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var amount = 100m;

            // Act
            var payment = new Payment(orderId, userId, amount);

            // Assert
            Assert.Equal(orderId, payment.OrderId);
            Assert.Equal(userId, payment.UserId);
            Assert.Equal(amount, payment.Amount);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            Assert.NotEqual(Guid.Empty, payment.Id);
            Assert.True(payment.CreatedAt <= DateTime.UtcNow);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Constructor_ShouldThrowArgumentException_WhenAmountIsInvalid(decimal amount)
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Payment(orderId, userId, amount));
        }

        [Fact]
        public void MarkAsCompleted_ShouldSetStatusToCompleted_WhenPending()
        {
            // Arrange
            var payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), 100);

            // Act
            payment.MarkAsCompleted();

            // Assert
            Assert.Equal(PaymentStatus.Completed, payment.Status);
        }

    }
}


