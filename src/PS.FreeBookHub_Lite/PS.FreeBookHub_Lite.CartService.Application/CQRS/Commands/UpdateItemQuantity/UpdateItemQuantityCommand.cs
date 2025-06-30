using MediatR;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommand : IRequest<Unit>
    {
        public Guid UserId { get; }
        public Guid BookId { get; }
        public int Quantity { get; }

        public UpdateItemQuantityCommand(Guid userId, Guid bookId, int quantity)
        {
            UserId = userId;
            BookId = bookId;
            Quantity = quantity;
        }
    }
}
