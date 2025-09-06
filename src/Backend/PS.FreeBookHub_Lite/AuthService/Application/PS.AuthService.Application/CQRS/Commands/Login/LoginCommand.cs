using MediatR;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
