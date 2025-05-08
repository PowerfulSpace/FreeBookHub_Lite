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

        public async Task<Cart?> GetCartAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}
