using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Common.Logging;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.LogoutAll
{
    public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<LogoutAllCommandHandler> _logger;

        public LogoutAllCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<LogoutAllCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(LoggerMessages.LogoutAllSessionsStarted, request.UserId);

            await _refreshTokenRepository.RevokeAllTokensForUserAsync(request.UserId, cancellationToken);

            _logger.LogInformation(LoggerMessages.LogoutAllSessionsCompleted, request.UserId);

            return Unit.Value;
        }
    }
}
