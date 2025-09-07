namespace PS.PaymentService.Domain.Exceptions.Base
{
    public abstract class PaymentServiceException : Exception
    {
        protected PaymentServiceException(string message) : base(message) { }
    }
}
