using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User
{
    public class UserNotFoundException : AuthServiceException
    {
        public Guid? UserId { get; }
        public string? Email { get; }

        public UserNotFoundException(Guid userId)
         : base($"User not found (ID: {userId})")
        {
            UserId = userId;
        }

        public UserNotFoundException(string email)
            : base($"User not found (email: {email})")
        {
            Email = email;
        }
    }
}
