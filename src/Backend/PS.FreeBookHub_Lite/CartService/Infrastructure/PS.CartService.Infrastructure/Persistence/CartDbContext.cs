using Microsoft.EntityFrameworkCore;
using PS.CartService.Domain.Entities;

namespace PS.CartService.Infrastructure.Persistence
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
