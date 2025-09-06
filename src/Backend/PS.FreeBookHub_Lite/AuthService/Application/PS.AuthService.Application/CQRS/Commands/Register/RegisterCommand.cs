using MediatR;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Register
{
    public class RegisterCommand : IRequest<AuthResponse>
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string? Role { get; init; }
    }
}
