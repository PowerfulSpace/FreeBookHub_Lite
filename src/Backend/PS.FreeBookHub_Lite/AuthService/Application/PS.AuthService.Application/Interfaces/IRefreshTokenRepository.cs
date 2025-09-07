using PS.AuthService.Domain.Entities;

namespace PS.AuthService.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct, bool asNoTracking = false);
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task UpdateAsync(RefreshToken token, CancellationToken ct);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken ct, bool asNoTracking = false);
        Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken ct);
    }
}
