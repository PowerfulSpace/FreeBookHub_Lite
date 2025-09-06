using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token
{
    public class InvalidTokenException : TokenException
    {
        public string Token { get; }

        public InvalidTokenException(string token)
            : base($"Invalid or expired refresh token")
        {
            Token = token;
        }
    }
}
