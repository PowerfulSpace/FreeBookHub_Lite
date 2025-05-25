using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.CartService.Application.Interfaces;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Infrastructure.Persistence.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext _context;

        public CartRepository(CartDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartAsync(Guid userId, CancellationToken cancellationToken, bool asNoTracking = false)
        {
            IQueryable<Cart> carts = _context.Carts.Include(c => c.Items);

            if (asNoTracking)
            {
                carts = carts.AsNoTracking();
            }

            return await carts.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        }

        public async Task AddAsync(Cart cart, CancellationToken cancellationToken)
        {
            await _context.Carts.AddAsync(cart, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Cart cart, CancellationToken cancellationToken)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
