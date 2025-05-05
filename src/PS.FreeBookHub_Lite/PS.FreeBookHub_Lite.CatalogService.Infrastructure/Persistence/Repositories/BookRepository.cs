using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.CatalogService.Application.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CatalogService.Infrastructure.Persistence.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly CatalogDbContext _context;

        public BookRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book is not null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}