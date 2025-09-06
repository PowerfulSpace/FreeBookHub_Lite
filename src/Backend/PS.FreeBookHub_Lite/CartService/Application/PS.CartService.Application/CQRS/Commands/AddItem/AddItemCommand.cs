using MediatR;

namespace PS.FreeBookHub_Lite.CartService.Application.CQRS.Commands.AddItem
{
    public class AddItemCommand : IRequest<Unit>
    {
        public Guid UserId { get; }
        public Guid BookId { get; }
        public int Quantity { get; }

        public AddItemCommand(Guid userId, Guid bookId, int quantity)
        {
            UserId = userId;
            BookId = bookId;
            Quantity = quantity;
        }
    }
}
