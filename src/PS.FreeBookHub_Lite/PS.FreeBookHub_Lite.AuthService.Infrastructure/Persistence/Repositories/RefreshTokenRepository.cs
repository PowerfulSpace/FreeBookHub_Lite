using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories
{
    internal class RefreshTokenRepository : IRefreshTokenRepository
    {
        public Task AddAsync(RefreshToken token, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(RefreshToken token, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
