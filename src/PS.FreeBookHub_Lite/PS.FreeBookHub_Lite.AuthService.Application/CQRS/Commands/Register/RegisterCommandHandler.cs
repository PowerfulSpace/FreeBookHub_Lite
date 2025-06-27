﻿using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User;

using RefreshTokenEntity = PS.FreeBookHub_Lite.AuthService.Domain.Entities.RefreshToken;

namespace PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            ILogger<RegisterCommandHandler> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Registration started for {Email}", request.Email);

            // Проверка существования пользователя
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct, asNoTracking: true);
            if (existingUser is not null)
            {
                throw new UserAlreadyExistsException(request.Email);
            }

            // Хеширование пароля
            var passwordHash = _passwordHasher.Hash(request.Password);

            // Парсинг роли
            var role = Enum.TryParse<UserRole>(request.Role, true, out var parsedRole)
                ? parsedRole
                : UserRole.User;

            // Создание пользователя
            var newUser = new User(request.Email, passwordHash, role);
            await _userRepository.AddAsync(newUser, ct);

            // Генерация токенов
            var accessToken = _tokenService.GenerateAccessToken(newUser);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            int refreshTokenExpiryDays = int.Parse(_configuration["Auth:JwtSettings:RefreshTokenExpiryDays"] ?? "7");

            var refreshToken = new RefreshTokenEntity(
                userId: newUser.Id,
                token: refreshTokenStr,
                expiresAt: DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            );

            await _refreshTokenRepository.AddAsync(refreshToken, ct);

            _logger.LogInformation("Registration successful for {Email} with UserId {UserId}", request.Email, newUser.Id);

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
