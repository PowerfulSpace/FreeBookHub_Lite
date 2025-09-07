using PS.AuthService.Domain.Exceptions.Token.Base;

namespace PS.AuthService.Domain.Exceptions.Token
{
    public class RevokedTokenException : TokenException
    {
        public string Token { get; }

        public RevokedTokenException(string token)
            : base($"Refresh token is already revoked")
        {
            Token = token;
        }
    }
}
