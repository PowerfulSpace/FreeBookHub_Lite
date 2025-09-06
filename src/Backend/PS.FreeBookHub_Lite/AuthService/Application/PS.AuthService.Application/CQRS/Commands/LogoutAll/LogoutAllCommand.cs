using MediatR;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.LogoutAll
{
    public class LogoutAllCommand : IRequest<Unit>
    {
        public Guid UserId { get; init; }
    }
}
