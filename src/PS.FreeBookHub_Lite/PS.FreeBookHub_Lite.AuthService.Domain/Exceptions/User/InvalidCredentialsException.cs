using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User
{
    public class InvalidCredentialsException : UserException
    {
        public InvalidCredentialsException()
            : base("Invalid email or password") { }
    }

}
