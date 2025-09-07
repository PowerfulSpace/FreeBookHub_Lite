using PS.PaymentService.Domain.Enums;
using PS.PaymentService.Domain.Exceptions.Base;

namespace PS.PaymentService.Domain.Exceptions.Payment
{
    public class InvalidPaymentStatusException : PaymentServiceException
    {
        public PaymentStatus CurrentStatus { get; }
        public PaymentStatus RequiredStatus { get; }

        public InvalidPaymentStatusException(PaymentStatus currentStatus, PaymentStatus requiredStatus)
            : base($"Cannot perform operation. Current status: {currentStatus}, required: {requiredStatus}.")
        {
            CurrentStatus = currentStatus;
            RequiredStatus = requiredStatus;
        }
    }
}
