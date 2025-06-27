using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.LogoutAll
{
    public class LogoutAllSessionsCommandHandler : IRequestHandler<LogoutAllSessionsCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<LogoutAllSessionsCommandHandler> _logger;

        public LogoutAllSessionsCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<LogoutAllSessionsCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(LogoutAllSessionsCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Revoking all sessions for user {UserId}", request.UserId);

            await _refreshTokenRepository.RevokeAllTokensForUserAsync(request.UserId, cancellationToken);

            _logger.LogInformation("All sessions revoked for user {UserId}", request.UserId);

            return Unit.Value;
        }
    }
}
