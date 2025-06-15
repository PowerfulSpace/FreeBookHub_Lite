using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Order
{
    public class InvalidOrderOperationException : OrderServiceException
    {
        public InvalidOrderOperationException(string message)
            : base(message) { }
    }
}
