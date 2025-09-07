namespace PS.AuthService.Application.DTOs
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
