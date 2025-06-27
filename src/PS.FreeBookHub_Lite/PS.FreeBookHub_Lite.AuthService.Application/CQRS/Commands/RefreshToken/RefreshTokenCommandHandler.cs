using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User;

using RefreshTokenEntity = PS.FreeBookHub_Lite.AuthService.Domain.Entities.RefreshToken;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            ITokenService tokenService,
            IConfiguration configuration,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
        }


        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refreshing token: {RefreshToken}", request.RefreshToken);

            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (existingToken is null || !existingToken.IsActive())
            {
                throw new InvalidTokenException(request.RefreshToken);
            }

            var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken, asNoTracking: true);
            if (user is null || !user.IsActive)
            {
                throw new UserByIdNotFoundException(existingToken.UserId);
            }

            // Revoke old token
            existingToken.Revoke();
            await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);

            _logger.LogInformation("Revoked old refresh token: {RefreshToken}", request.RefreshToken);

            // Generate new token
            var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:JwtSettings:RefreshTokenExpiryDays"] ?? "7");

            var newRefreshToken = new RefreshTokenEntity(
                userId: user.Id,
                token: newRefreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user);
            int accessTokenExpiryMinutes = int.Parse(_configuration["Auth:JwtSettings:AccessTokenExpiryMinutes"] ?? "15");

            _logger.LogInformation("Issued new refresh token for user {UserId}", user.Id);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenStr,
                ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };
        }

    }
}
