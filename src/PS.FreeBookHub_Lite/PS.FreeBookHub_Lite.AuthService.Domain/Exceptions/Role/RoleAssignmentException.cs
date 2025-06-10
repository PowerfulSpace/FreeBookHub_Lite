using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role
{
    public class RoleAssignmentException : AuthServiceException
    {
        public RoleAssignmentException(string message) : base(message) { }
    }
}
