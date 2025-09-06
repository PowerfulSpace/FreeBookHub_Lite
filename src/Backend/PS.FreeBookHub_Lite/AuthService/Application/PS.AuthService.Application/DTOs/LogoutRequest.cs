namespace PS.FreeBookHub_Lite.AuthService.Application.DTOs
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
