using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct, bool asNoTracking = false);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct, bool asNoTracking = false);
        Task AddAsync(User user, CancellationToken ct);
        Task UpdateAsync(User user, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
