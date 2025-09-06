using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token.Base
{
    public abstract class TokenException : AuthServiceException
    {
        protected TokenException(string message) : base(message) { }
    }
}
