using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions
{
    public class CartNotFoundException : CartServiceException
    {
        public Guid UserId { get; }

        public CartNotFoundException(Guid userId)
            : base($"Cart for user {userId} not found")
        {
            UserId = userId;
        }
    }
}
