namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart.Base
{
    public abstract class CartServiceException : Exception
    {
        protected CartServiceException(string message) : base(message) { }
    }
}
