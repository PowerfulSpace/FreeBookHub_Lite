using MediatR;
using PS.AuthService.Application.DTOs;

namespace PS.AuthService.Application.CQRS.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
