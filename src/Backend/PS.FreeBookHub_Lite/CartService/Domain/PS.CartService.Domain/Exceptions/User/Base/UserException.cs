namespace PS.FreeBookHub_Lite.CartService.Domain.Exceptions.User.Base
{
    public abstract class UserException : Exception
    {
        protected UserException(string message) : base(message) { }
    }
}
