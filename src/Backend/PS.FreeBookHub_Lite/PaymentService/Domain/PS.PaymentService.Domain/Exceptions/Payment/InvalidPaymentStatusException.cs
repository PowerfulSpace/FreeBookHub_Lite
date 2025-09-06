using PS.FreeBookHub_Lite.PaymentService.Domain.Enums;
using PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.Payment
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
