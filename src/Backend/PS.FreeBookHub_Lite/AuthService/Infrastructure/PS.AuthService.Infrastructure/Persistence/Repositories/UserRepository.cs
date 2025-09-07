using Microsoft.EntityFrameworkCore;
using PS.AuthService.Application.Interfaces;
using PS.AuthService.Domain.Entities;

namespace PS.AuthService.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct, bool asNoTracking = false)
        {
            IQueryable<User> users = _context.Users;

            if (asNoTracking)
                users = users.AsNoTracking();

            return await users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct, bool asNoTracking = false)
        {
            IQueryable<User> users = _context.Users;

            if (asNoTracking)
                users = users.AsNoTracking();

            return await users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            _context.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync(ct);
        }
    }
}
