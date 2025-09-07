using PS.AuthService.Domain.Exceptions.Role.Base;

namespace PS.AuthService.Domain.Exceptions.Role
{
    public class RoleAssignmentException : RoleException
    {
        public RoleAssignmentException(string message) : base(message) { }
    }
}
