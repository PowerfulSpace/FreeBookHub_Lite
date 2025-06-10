using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User
{
    public class UserByIdNotFoundException : UserException
    {
        public Guid UserId { get; }

        public UserByIdNotFoundException(Guid userId)
            : base($"User not found (ID: {userId})")
        {
            UserId = userId;
        }
    }
}
