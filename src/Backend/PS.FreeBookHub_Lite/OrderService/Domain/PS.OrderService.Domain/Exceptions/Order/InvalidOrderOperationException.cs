using PS.OrderService.Domain.Exceptions.Base;

namespace PS.OrderService.Domain.Exceptions.Order
{
    public class InvalidOrderOperationException : OrderServiceException
    {
        public InvalidOrderOperationException(string message)
            : base(message) { }
    }
}
