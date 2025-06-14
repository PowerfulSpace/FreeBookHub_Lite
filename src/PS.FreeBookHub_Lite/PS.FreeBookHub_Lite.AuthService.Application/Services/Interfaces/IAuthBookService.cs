﻿using PS.FreeBookHub_Lite.AuthService.Application.DTOs;

namespace PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces
{
    public interface IAuthBookService
    {
        Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct);
        Task LogoutAllSessionsAsync(Guid userId, CancellationToken ct);
        Task LogoutCurrentSessionAsync(LogoutRequest request, CancellationToken ct);
    }
}
