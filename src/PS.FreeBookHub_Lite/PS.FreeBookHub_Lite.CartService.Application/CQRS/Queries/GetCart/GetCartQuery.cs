using MediatR;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Cart;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Queries.GetCart
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
