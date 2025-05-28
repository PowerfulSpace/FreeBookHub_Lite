using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;

        public RefreshTokenRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token, ct);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken ct)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsActive())
                .ToListAsync(ct);
        }
    }
}
