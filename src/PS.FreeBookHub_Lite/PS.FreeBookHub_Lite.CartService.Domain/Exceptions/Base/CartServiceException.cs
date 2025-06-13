namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Base
{
    public abstract class CartServiceException : Exception
    {
        protected CartServiceException(string message) : base(message) { }
    }
}
