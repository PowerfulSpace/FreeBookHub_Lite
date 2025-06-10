using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role
{
    public class InvalidRolePromotionException : RoleException
    {
        public InvalidRolePromotionException(string message) : base(message) { }
    }
}
