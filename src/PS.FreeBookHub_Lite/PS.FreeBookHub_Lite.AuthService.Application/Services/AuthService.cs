using Microsoft.Extensions.Configuration;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;

namespace PS.FreeBookHub_Lite.AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (existingUser is not null)
                throw new InvalidOperationException("User with this email already exists.");

            var passwordHash = await _passwordHasher.Hash(request.Password);

            var role = Enum.TryParse<UserRole>(request.Role, true, out var parsedRole)
                ? parsedRole
                : UserRole.User;

            var newUser = new User(request.Email, passwordHash, role);
            await _userRepository.AddAsync(newUser, ct);

            var accessToken = _tokenService.GenerateAccessToken(newUser);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:RefreshTokenExpiryDays"] ?? "7");
            var refreshToken = new RefreshToken(
                userId: newUser.Id,
                token: refreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(refreshToken, ct);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenStr,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Auth:AccessTokenExpiryMinutes"] ?? "15")
                )
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (user is null)
                throw new InvalidOperationException("Invalid email or password.");

            if (!user.IsActive)
                throw new InvalidOperationException("User account is deactivated.");

            var isPasswordValid = await _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new InvalidOperationException("Invalid email or password.");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:RefreshTokenExpiryDays"] ?? "7");
            var refreshToken = new RefreshToken(
                userId: user.Id,
                token: refreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(refreshToken, ct);

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
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
            if (existingToken is null || !existingToken.IsActive())
                throw new InvalidOperationException("Invalid or expired refresh token.");

            var user = await _userRepository.GetByIdAsync(existingToken.UserId, ct);
            if (user is null || !user.IsActive)
                throw new InvalidOperationException("User not found or inactive.");

            // Отозвать старый токен
            existingToken.Revoke();
            await _refreshTokenRepository.UpdateAsync(existingToken, ct);

            // Создать новый
            var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:RefreshTokenExpiryDays"] ?? "7");

            var newRefreshToken = new RefreshToken(
                userId: user.Id,
                token: newRefreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(newRefreshToken, ct);

            var accessToken = _tokenService.GenerateAccessToken(user);
            int accessTokenExpiryMinutes = int.Parse(_configuration["Auth:AccessTokenExpiryMinutes"] ?? "15");

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenStr,
                ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            };
        }

        public async Task LogoutAllSessionsAsync(Guid userId, CancellationToken ct)
        {
            var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(userId, ct);
            if (activeTokens.Count == 0)
                return;

            foreach (var token in activeTokens)
            {
                token.Revoke();
                await _refreshTokenRepository.UpdateAsync(token, ct);
            }
        }

        public async Task LogoutCurrentSessionAsync(string refreshToken, CancellationToken ct)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
            if (token is null || !token.IsActive())
                return;

            token.Revoke();
            await _refreshTokenRepository.UpdateAsync(token, ct);
        }
    }
}
