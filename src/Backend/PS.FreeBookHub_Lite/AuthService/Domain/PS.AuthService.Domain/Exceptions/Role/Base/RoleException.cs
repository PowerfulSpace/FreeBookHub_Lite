using PS.AuthService.Domain.Exceptions.Base;

namespace PS.AuthService.Domain.Exceptions.Role.Base
{
    public abstract class RoleException : AuthServiceException
    {
        protected RoleException(string message) : base(message) { }
    }
}
