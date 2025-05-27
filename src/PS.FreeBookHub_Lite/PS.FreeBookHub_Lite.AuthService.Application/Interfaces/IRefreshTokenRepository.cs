using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task UpdateAsync(RefreshToken token, CancellationToken ct);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken ct);
    }
}
