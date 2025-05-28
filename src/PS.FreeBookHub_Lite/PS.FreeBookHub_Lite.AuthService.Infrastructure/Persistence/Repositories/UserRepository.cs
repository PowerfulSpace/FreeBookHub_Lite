using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is not null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}
