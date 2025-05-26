namespace PS.FreeBookHub_Lite.AuthService.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
