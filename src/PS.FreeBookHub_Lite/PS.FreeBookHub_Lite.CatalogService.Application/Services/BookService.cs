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

        public async Task<IEnumerable<BookResponse>> GetAllBooksAsync(CancellationToken cancellationToken)
        {
            var books = await _repository.GetAllAsync(cancellationToken);
            return books.Adapt<IEnumerable<BookResponse>>();
        }

        public async Task<BookResponse?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var book = await _repository.GetByIdAsync(id, cancellationToken);
            return book?.Adapt<BookResponse>();
        }

        public async Task<BookResponse> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken)
        {
            var book = request.Adapt<Book>();
            await _repository.AddAsync(book, cancellationToken);
            return book.Adapt<BookResponse>();
        }

        public async Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken)
        {
            var book = await _repository.GetByIdAsync(id, cancellationToken);
            if (book == null) return false;

            await _repository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateBookAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null) return false;

            request.Adapt(existing);
            await _repository.UpdateAsync(existing, cancellationToken);

            return true;
        }

        public async Task<decimal?> GetBookPriceAsync(Guid id, CancellationToken cancellationToken)
        {
            var book = await _repository.GetByIdAsync(id, cancellationToken);
            return book?.Price;
        }
    }
}
