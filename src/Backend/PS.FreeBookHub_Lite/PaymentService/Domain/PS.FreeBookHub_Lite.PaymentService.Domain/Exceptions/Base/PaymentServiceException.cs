namespace PS.FreeBookHub_Lite.PaymentService.Domain.Exceptions.Base
{
    public abstract class PaymentServiceException : Exception
    {
        protected PaymentServiceException(string message) : base(message) { }
    }
}
