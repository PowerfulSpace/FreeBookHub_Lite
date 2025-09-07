namespace PS.OrderService.Domain.Exceptions.Base
{
    public abstract class OrderServiceException : Exception
    {
        protected OrderServiceException(string message) : base(message) { }
    }
}
