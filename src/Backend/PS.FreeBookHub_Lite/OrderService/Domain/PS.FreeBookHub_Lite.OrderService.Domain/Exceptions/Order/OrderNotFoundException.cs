using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Order
{
    public class OrderNotFoundException : OrderServiceException
    {
        public Guid OrderId { get; }
        public OrderNotFoundException(Guid orderId)
            : base($"Order not found (ID: {orderId})")
        {
            OrderId = orderId;
        }
    }
}
