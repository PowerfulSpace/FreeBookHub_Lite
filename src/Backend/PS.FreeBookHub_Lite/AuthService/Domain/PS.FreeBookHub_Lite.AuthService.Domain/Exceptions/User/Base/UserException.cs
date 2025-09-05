using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User.Base
{
    public abstract class UserException : AuthServiceException
    {
        protected UserException(string message) : base(message) { }
    }
}
