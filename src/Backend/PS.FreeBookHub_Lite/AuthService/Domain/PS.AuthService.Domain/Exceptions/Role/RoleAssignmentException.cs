using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role
{
    public class RoleAssignmentException : RoleException
    {
        public RoleAssignmentException(string message) : base(message) { }
    }
}
