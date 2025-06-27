using MediatR;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.LogoutAll
{
    public class LogoutAllSessionsCommand : IRequest<Unit>
    {
        public Guid UserId { get; init; }
    }
}
