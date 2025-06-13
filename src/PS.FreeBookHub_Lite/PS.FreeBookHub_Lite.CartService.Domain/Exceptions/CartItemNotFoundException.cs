using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions
{
    public class CartItemNotFoundException : CartServiceException
    {
        public Guid BookId { get; }
        public Guid UserId { get; }

        public CartItemNotFoundException(Guid userId, Guid bookId)
            : base($"Товар с ID {bookId} не найден в корзине пользователя {userId}")
        {
            UserId = userId;
            BookId = bookId;
        }
    }
}
