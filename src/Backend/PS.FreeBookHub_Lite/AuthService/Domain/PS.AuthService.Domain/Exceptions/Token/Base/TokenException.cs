using PS.AuthService.Domain.Exceptions.Base;

namespace PS.AuthService.Domain.Exceptions.Token.Base
{
    public abstract class TokenException : AuthServiceException
    {
        protected TokenException(string message) : base(message) { }
    }
}
