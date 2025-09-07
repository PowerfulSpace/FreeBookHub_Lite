using PS.PaymentService.Domain.Exceptions.Base;

namespace PS.PaymentService.Domain.Exceptions.Payment
{
    public class UnauthorizedPaymentAccessException : PaymentServiceException
    {
        public Guid PaymentId { get; }
        public Guid UserId { get; }

        public UnauthorizedPaymentAccessException(Guid paymentId, Guid userId)
            : base($"User {userId} is not authorized to access payment {paymentId}.")
        {
            PaymentId = paymentId;
            UserId = userId;
        }
    }
}
