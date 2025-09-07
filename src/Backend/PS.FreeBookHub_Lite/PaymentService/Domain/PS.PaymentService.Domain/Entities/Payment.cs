using PS.PaymentService.Domain.Enums;
using PS.PaymentService.Domain.Exceptions.Payment;

namespace PS.PaymentService.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }

        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected Payment() { }

        public Payment(Guid orderId, Guid userId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsCompleted()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidPaymentStatusException(Status, PaymentStatus.Pending);

            Status = PaymentStatus.Completed;
        }

        public void MarkAsFailed()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidPaymentStatusException(Status, PaymentStatus.Pending);

            Status = PaymentStatus.Failed;
        }
    }
}
