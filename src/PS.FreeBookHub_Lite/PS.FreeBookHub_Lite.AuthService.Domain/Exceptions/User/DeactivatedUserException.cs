using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User
{
    public class DeactivatedUserException : AuthServiceException
    {
        public Guid UserId { get; }

        public DeactivatedUserException(Guid userId)
            : base($"User account (ID: {userId}) is deactivated")
        {
            UserId = userId;
        }
    }
}
