using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Common.Logging;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User;

using RefreshTokenEntity = PS.FreeBookHub_Lite.AuthService.Domain.Entities.RefreshToken;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }


        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.LoginStarted, request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken, asNoTracking: true);
            if (user is null)
            {
                throw new InvalidCredentialsException();
            }

            if (!user.IsActive)
            {
                throw new DeactivatedUserException(user.Id);
            }

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new InvalidCredentialsException();
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:JwtSettings:RefreshTokenExpiryDays"] ?? "7");
            var refreshToken = new RefreshTokenEntity(
                userId: user.Id,
                token: refreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            _logger.LogInformation(LoggerMessages.LoginSuccess, user.Email, user.Id);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenStr,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Auth:JwtSettings:AccessTokenExpiryMinutes"] ?? "15")
                )
            };
        }
    }
}