using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Common.Logging;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;

namespace PS.FreeBookHub_Lite.AuthService.Application.Services
{
    public class AuthBookService : IAuthBookService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthBookService> _logger;


        public AuthBookService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            ILogger<AuthBookService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct)
        {
            _logger.LogInformation(LoggerMessages.RegistrationStarted, request.Email);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct, asNoTracking: true);
            if (existingUser is not null)
            {
                _logger.LogWarning(LoggerMessages.RegistrationUserAlreadyExists, request.Email);
                throw new InvalidOperationException("User with this email already exists.");
            }
                
            var passwordHash = _passwordHasher.Hash(request.Password);

            var role = Enum.TryParse<UserRole>(request.Role, true, out var parsedRole)
                ? parsedRole
                : UserRole.User;

            var newUser = new User(request.Email, passwordHash, role);
            await _userRepository.AddAsync(newUser, ct);

            var accessToken = _tokenService.GenerateAccessToken(newUser);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:JwtSettings:RefreshTokenExpiryDays"] ?? "7");
            var refreshToken = new RefreshToken(
                userId: newUser.Id,
                token: refreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(refreshToken, ct);

            _logger.LogInformation(LoggerMessages.RegistrationSuccess, request.Email, newUser.Id);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenStr,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Auth:JwtSettings:AccessTokenExpiryMinutes"] ?? "15")
                )
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            _logger.LogInformation(LoggerMessages.LoginStarted, request.Email);
            var user = await _userRepository.GetByEmailAsync(request.Email, ct, asNoTracking: true);
            if (user is null)
            {
                _logger.LogWarning(LoggerMessages.LoginUserNotFound, request.Email);
                throw new InvalidOperationException("Invalid email or password.");
            }
                
            if (!user.IsActive)
            {
                _logger.LogWarning(LoggerMessages.LoginUserInactive, request.Email);
                throw new InvalidOperationException("User account is deactivated.");
            }

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning(LoggerMessages.LoginInvalidPassword, request.Email);
                throw new InvalidOperationException("Invalid email or password.");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:JwtSettings:RefreshTokenExpiryDays"] ?? "7");
            var refreshToken = new RefreshToken(
                userId: user.Id,
                token: refreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(refreshToken, ct);

            _logger.LogInformation(LoggerMessages.LoginSuccess, user.Email, user.Id);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenStr,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Auth:AccessTokenExpiryMinutes"] ?? "15")
                )
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct)
        {
            _logger.LogInformation(LoggerMessages.RefreshStarted, request.RefreshToken);
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
            if (existingToken is null || !existingToken.IsActive())
            {
                _logger.LogWarning(LoggerMessages.RefreshTokenInvalidOrExpired, request.RefreshToken);
                throw new InvalidOperationException("Invalid or expired refresh token.");
            }

            var user = await _userRepository.GetByIdAsync(existingToken.UserId, ct, asNoTracking: true);
            if (user is null || !user.IsActive)
            {
                _logger.LogWarning(LoggerMessages.RefreshUserNotFoundOrInactive, existingToken.UserId);
                throw new InvalidOperationException("User not found or inactive.");
            }

            // Отозвать старый токен
            existingToken.Revoke();
            await _refreshTokenRepository.UpdateAsync(existingToken, ct);

            _logger.LogInformation(LoggerMessages.RefreshOldTokenRevoked, request.RefreshToken);

            // Создать новый
            var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:JwtSettings:RefreshTokenExpiryDays"] ?? "7");

            var newRefreshToken = new RefreshToken(
                userId: user.Id,
                token: newRefreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(newRefreshToken, ct);

            var accessToken = _tokenService.GenerateAccessToken(user);
            int accessTokenExpiryMinutes = int.Parse(_configuration["Auth:JwtSettings:AccessTokenExpiryMinutes"] ?? "15");

            _logger.LogInformation(LoggerMessages.RefreshNewTokenIssued, user.Id);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenStr,
                ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };
        }

        public async Task LogoutAllSessionsAsync(Guid userId, CancellationToken ct)
        {
            _logger.LogInformation(LoggerMessages.LogoutAllSessionsStarted, userId);

            await _refreshTokenRepository.RevokeAllTokensForUserAsync(userId, ct);

            _logger.LogInformation(LoggerMessages.LogoutAllSessionsCompleted, userId);
        }

        public async Task LogoutCurrentSessionAsync(LogoutRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation(LoggerMessages.LogoutSessionStarted, request.RefreshToken);

            var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

            if (token is null)
            {
                _logger.LogWarning(LoggerMessages.LogoutTokenNotFound, request.RefreshToken);
                throw new InvalidOperationException("Refresh token not found");
            }

            if (!token.IsActive())
            {
                _logger.LogWarning(LoggerMessages.LogoutTokenAlreadyRevoked, request.RefreshToken);
                throw new InvalidOperationException("Refresh token is already revoked or expired");
            }

            token.Revoke();
            await _refreshTokenRepository.UpdateAsync(token, ct);
            _logger.LogInformation(LoggerMessages.LogoutTokenRevoked, request.RefreshToken);
        }
    }
}
