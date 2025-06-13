using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions
{
    public class CartNotFoundException : CartServiceException
    {
        public Guid UserId { get; }

        public CartNotFoundException(Guid userId)
            : base($"Корзина для пользователя {userId} не найдена")
        {
            UserId = userId;
        }
    }
}
