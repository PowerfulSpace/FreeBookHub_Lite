using PS.AuthService.Domain.Exceptions.Role.Base;

namespace PS.AuthService.Domain.Exceptions.Role
{
    public class InvalidRolePromotionException : RoleException
    {
        public InvalidRolePromotionException(string message) : base(message) { }
    }
}
