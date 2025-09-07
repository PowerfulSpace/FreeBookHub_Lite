using MediatR;
using Microsoft.Extensions.Logging;
using PS.AuthService.Application.Interfaces;
using PS.AuthService.Common.Logging;
using PS.AuthService.Domain.Exceptions.Token;

namespace PS.AuthService.Application.CQRS.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<LogoutCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(LoggerMessages.LogoutSessionStarted, request.RefreshToken);

            var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (token is null)
            {
                throw new TokenNotFoundException(request.RefreshToken);
            }

            if (!token.IsActive())
            {
                throw new RevokedTokenException(request.RefreshToken);
            }

            token.Revoke();
            await _refreshTokenRepository.UpdateAsync(token, cancellationToken);

            _logger.LogInformation(LoggerMessages.LogoutTokenRevoked, request.RefreshToken);

            return Unit.Value;
        }
    }
}
