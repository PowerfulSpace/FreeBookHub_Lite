using MediatR;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<AuthResponse>
    {
        public string RefreshToken { get; init; } = null!;
    }
}
