using PS.FreeBookHub_Lite.PaymentService.Domain.Enums;

namespace PS.FreeBookHub_Lite.PaymentService.Domain.Entities
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
                throw new InvalidOperationException("Only pending payments can be completed.");

            Status = PaymentStatus.Completed;
        }

        public void MarkAsFailed()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be marked as failed.");

            Status = PaymentStatus.Failed;
        }
    }
}
