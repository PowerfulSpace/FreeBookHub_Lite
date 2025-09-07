using MediatR;
using PS.AuthService.Application.DTOs;

namespace PS.AuthService.Application.CQRS.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<AuthResponse>
    {
        public string RefreshToken { get; init; } = null!;
    }
}
