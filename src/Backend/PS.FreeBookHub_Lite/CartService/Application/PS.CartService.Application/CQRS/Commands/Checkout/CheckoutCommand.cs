using MediatR;
using PS.CartService.Application.DTOs.Order;

namespace PS.CartService.Application.CQRS.Commands.Checkout
{
    public class CheckoutCommand : IRequest<OrderResponse>
    {
        public Guid UserId { get; }
        public string ShippingAddress { get; }

        public CheckoutCommand(Guid userId, string shippingAddress)
        {
            UserId = userId;
            ShippingAddress = shippingAddress;
        }
    }
}
