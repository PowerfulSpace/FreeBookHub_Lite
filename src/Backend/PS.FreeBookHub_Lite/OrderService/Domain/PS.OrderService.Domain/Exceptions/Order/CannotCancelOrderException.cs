using PS.OrderService.Domain.Enums;
using PS.OrderService.Domain.Exceptions.Base;

namespace PS.OrderService.Domain.Exceptions.Order
{
    public class CannotCancelOrderException : OrderServiceException
    {
        public Guid OrderId { get; }
        public OrderStatus CurrentStatus { get; }

        public CannotCancelOrderException(Guid orderId, OrderStatus status)
            : base($"Cannot cancel order {orderId} with status '{status}'.")
        {
            OrderId = orderId;
            CurrentStatus = status;
        }
    }
}
