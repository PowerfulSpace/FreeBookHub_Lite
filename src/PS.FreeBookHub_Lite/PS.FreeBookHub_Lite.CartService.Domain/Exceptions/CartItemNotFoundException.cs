using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions
{
    public class CartItemNotFoundException : CartServiceException
    {
        public Guid BookId { get; }
        public Guid UserId { get; }

        public CartItemNotFoundException(Guid userId, Guid bookId)
            : base($"Item with ID {bookId} not found in user {userId}'s cart")
        {
            UserId = userId;
            BookId = bookId;
        }
    }
}
