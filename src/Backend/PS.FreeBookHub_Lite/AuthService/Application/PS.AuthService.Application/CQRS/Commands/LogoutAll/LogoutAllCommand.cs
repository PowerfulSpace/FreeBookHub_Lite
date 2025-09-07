using MediatR;

namespace PS.AuthService.Application.CQRS.Commands.LogoutAll
{
    public class LogoutAllCommand : IRequest<Unit>
    {
        public Guid UserId { get; init; }
    }
}
