using Mapster;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;
using PS.FreeBookHub_Lite.CatalogService.Application.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _repository.GetAllAsync();
            return books.Adapt<IEnumerable<BookDto>>();
        }

        public async Task<BookDto?> GetBookByIdAsync(Guid id)
        {
            var book = await _repository.GetByIdAsync(id);
            return book?.Adapt<BookDto>();
        }

        public async Task<BookDto> CreateBookAsync(CreateBookRequest request)
        {
            var book = request.Adapt<Book>();
            await _repository.AddAsync(book);
            return book.Adapt<BookDto>();
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateBookAsync(Guid id, UpdateBookRequest request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            request.Adapt(existing);
            await _repository.UpdateAsync(existing);

            return true;
        }

        public async Task<decimal?> GetBookPriceAsync(Guid id)
        {
            var book = await _repository.GetByIdAsync(id);
            return book?.Price;
        }
    }
}
