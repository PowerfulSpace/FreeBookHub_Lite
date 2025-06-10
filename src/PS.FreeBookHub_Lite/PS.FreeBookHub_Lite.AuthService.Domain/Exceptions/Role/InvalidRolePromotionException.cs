namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role
{
    public class InvalidRolePromotionException : RoleAssignmentException
    {
        public InvalidRolePromotionException(string message) : base(message) { }
    }
}
