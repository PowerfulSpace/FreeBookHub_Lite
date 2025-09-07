namespace PS.AuthService.Application.DTOs
{
    public class RegisterUserRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Role { get; set; } // по умолчанию "User", может быть null
    }
}
