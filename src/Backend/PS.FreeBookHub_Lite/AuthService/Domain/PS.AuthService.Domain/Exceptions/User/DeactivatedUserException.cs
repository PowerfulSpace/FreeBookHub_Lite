using PS.AuthService.Domain.Exceptions.User.Base;

namespace PS.AuthService.Domain.Exceptions.User
{
    public class DeactivatedUserException : UserException
    {
        public Guid UserId { get; }

        public DeactivatedUserException(Guid userId)
            : base($"User account (ID: {userId}) is deactivated")
        {
            UserId = userId;
        }
    }
}
