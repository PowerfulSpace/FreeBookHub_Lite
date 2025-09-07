using MediatR;

namespace PS.AuthService.Application.CQRS.Commands.Logout
{
    public class LogoutCommand : IRequest<Unit>
    {
        public string RefreshToken { get; init; } = null!;
    }
}
