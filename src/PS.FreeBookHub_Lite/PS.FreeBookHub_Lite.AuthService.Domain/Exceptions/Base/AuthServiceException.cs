namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base
{
    public abstract class AuthServiceException : Exception
    {
        protected AuthServiceException(string message) : base(message) { }
    }
}
