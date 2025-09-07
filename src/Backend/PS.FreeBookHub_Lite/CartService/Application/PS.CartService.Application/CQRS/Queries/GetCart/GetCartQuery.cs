using MediatR;
using PS.CartService.Application.DTOs.Cart;

namespace PS.CartService.Application.CQRS.Queries.GetCart
{
    public class GetCartQuery : IRequest<CartResponse>
    {
        public Guid UserId { get; }

        public GetCartQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
