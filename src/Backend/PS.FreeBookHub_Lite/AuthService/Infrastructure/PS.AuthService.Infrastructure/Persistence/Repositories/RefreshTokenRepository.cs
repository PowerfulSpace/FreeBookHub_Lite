using Microsoft.EntityFrameworkCore;
using PS.AuthService.Application.Interfaces;
using PS.AuthService.Domain.Entities;

namespace PS.AuthService.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;

        public RefreshTokenRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct, bool asNoTracking = false)
        {
            IQueryable<RefreshToken> tokens = _context.RefreshTokens;

            if (asNoTracking)
                tokens = tokens.AsNoTracking();

            return await tokens.FirstOrDefaultAsync(rt => rt.Token == token, ct);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken ct)
        {
            _context.Attach(token);
            _context.Entry(token).State = EntityState.Modified;
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken ct, bool asNoTracking = false)
        {
            // Переписал,т.к SQLite In-Memory + EF Core не поддерживает работу с деревьями 
            //IQueryable<RefreshToken> tokens = _context.RefreshTokens
            //    .Where(rt => rt.UserId == userId && rt.IsActive());

            IQueryable<RefreshToken> tokens = _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && DateTime.UtcNow < rt.ExpiresAt);

            if (asNoTracking)
                tokens = tokens.AsNoTracking();

            return await tokens.ToListAsync(ct);
        }


        public async Task RevokeAllTokensForUserAsync(Guid userId, CancellationToken ct)
        {
            //Временно заменена логика, т.к тест не проходит, из-за дерева выражений

            //await _context.RefreshTokens
            //.Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            //.ExecuteUpdateAsync(setters =>
            //    setters.SetProperty(rt => rt.IsRevoked, true),
            //ct);

            var now = DateTime.UtcNow;

            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > now)
                .ToListAsync(ct);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
