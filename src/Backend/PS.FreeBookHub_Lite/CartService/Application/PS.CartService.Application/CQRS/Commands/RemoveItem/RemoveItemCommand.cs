using MediatR;

namespace PS.CartService.Application.CQRS.Commands.RemoveItem
{
    public class RemoveItemCommand : IRequest<Unit>
    {
        public Guid UserId { get; }
        public Guid BookId { get; }

        public RemoveItemCommand(Guid userId, Guid bookId)
        {
            UserId = userId;
            BookId = bookId;
        }
    }
}
