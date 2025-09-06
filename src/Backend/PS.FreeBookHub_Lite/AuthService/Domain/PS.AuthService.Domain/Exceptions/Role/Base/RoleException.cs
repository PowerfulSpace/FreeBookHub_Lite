using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role.Base
{
    public abstract class RoleException : AuthServiceException
    {
        protected RoleException(string message) : base(message) { }
    }
}
