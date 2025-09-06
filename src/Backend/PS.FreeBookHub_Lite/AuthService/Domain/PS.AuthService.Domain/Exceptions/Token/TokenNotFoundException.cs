using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token.Base;

namespace PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token
{
    public class TokenNotFoundException : TokenException
    {
        public string Token { get; }

        public TokenNotFoundException(string token)
            : base($"Refresh token not found")
        {
            Token = token;
        }
    }
}
