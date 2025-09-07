using PS.AuthService.Domain.Exceptions.Base;

namespace PS.AuthService.Domain.Exceptions.User.Base
{
    public abstract class UserException : AuthServiceException
    {
        protected UserException(string message) : base(message) { }
    }
}
