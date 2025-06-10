using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User
{
    public class UserAlreadyExistsException : UserException
    {
        public string Email { get; }

        public UserAlreadyExistsException(string email)
            : base($"User with email '{email}' already exists")
        {
            Email = email;
        }
    }
}
