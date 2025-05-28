using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
