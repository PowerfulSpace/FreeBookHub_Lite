using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User
{
    public class InvalidCredentialsException : AuthServiceException
    {
        public InvalidCredentialsException()
            : base("Invalid email or password") { }
    }

}
