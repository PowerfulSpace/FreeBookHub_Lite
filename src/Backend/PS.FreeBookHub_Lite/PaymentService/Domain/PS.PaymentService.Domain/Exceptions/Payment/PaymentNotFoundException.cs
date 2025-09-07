using PS.PaymentService.Domain.Exceptions.Base;

namespace PS.PaymentService.Domain.Exceptions.Payment
{
    public class PaymentNotFoundException : PaymentServiceException
    {
        public Guid PaymentId { get; }
        public PaymentNotFoundException(Guid paymentId)
            : base($"Payment not found: {paymentId}")
        {
            PaymentId = paymentId;
        }
    }
}
