using MediatR;
using PS.AuthService.Application.DTOs;

namespace PS.AuthService.Application.CQRS.Commands.Register
{
    public class RegisterCommand : IRequest<AuthResponse>
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string? Role { get; init; }
    }
}
