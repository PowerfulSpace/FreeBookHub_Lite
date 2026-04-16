using PS.PaymentService.Domain.Entities;
using PS.PaymentService.Domain.Enums;

namespace PS.PaymentService.UnitTests.Domain
{
    public class PaymentTests
    {
        [Fact]
        public void Constructor_ShouldCreatePayment_WhenDataIsValid()
        {
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var amount = 100m;

            var payment = new Payment(orderId, userId, amount);

            Assert.Equal(orderId, payment.OrderId);
            Assert.Equal(userId, payment.UserId);
            Assert.Equal(amount, payment.Amount);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            Assert.NotEqual(Guid.Empty, payment.Id);
            Assert.True(payment.CreatedAt <= DateTime.UtcNow);
        }

    }
}