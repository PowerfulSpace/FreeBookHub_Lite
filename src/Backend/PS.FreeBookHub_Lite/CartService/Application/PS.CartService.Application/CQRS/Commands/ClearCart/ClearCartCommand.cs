using MediatR;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<Unit>
    {
        public Guid UserId { get; }

        public ClearCartCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
