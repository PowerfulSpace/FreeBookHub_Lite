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

        public async Task AddOrUpdateItemAsync(Guid userId, Guid bookId, int quantity, decimal price)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
            {
                cart = new Cart(userId);
                _context.Carts.Add(cart);
            }

            cart.AddItem(bookId, quantity, price);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(Guid userId, Guid bookId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
                return;

            cart.RemoveItem(bookId);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
                return;

            cart.Clear();
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(Guid userId, Guid bookId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
                throw new InvalidOperationException("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.BookId == bookId);

            if (item is null)
                throw new InvalidOperationException("Item not found in cart");

            item.Quantity = quantity;

            await _context.SaveChangesAsync();
        }
    }
}
