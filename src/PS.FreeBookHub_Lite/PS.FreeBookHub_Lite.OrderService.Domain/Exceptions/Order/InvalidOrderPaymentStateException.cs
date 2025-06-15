using PS.FreeBookHub_Lite.OrderService.Domain.Enums;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Order
{
    public class InvalidOrderPaymentStateException : OrderServiceException
    {
        public Guid OrderId { get; }
        public OrderStatus CurrentStatus { get; }

        public InvalidOrderPaymentStateException(Guid orderId, OrderStatus status)
            : base($"Cannot mark order {orderId} as paid. Current status: '{status}'.")
        {
            OrderId = orderId;
            CurrentStatus = status;
        }
    }
}
