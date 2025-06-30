using MediatR;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.Checkout
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
