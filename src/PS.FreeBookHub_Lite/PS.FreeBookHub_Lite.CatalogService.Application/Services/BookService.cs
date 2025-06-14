using Mapster;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;
using PS.FreeBookHub_Lite.CatalogService.Application.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Common;
using PS.FreeBookHub_Lite.CatalogService.Domain.Entities;
using PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository repository, ILogger<BookService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<BookResponse>> GetAllBooksAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetAllBooksStarted);

            var books = await _repository.GetAllAsync(cancellationToken);

            var response = books.Adapt<IEnumerable<BookResponse>>();

            _logger.LogInformation(LoggerMessages.GetAllBooksSuccess, response.Count());

            return response;
        }

        public async Task<BookResponse?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetBookByIdStarted, id);

            var book = await _repository.GetByIdAsync(id, cancellationToken);

            if (book == null)
            {
                _logger.LogWarning(LoggerMessages.GetBookByIdNotFound, id);
                throw new BookNotFoundException(id);
            }

            _logger.LogInformation(LoggerMessages.GetBookByIdSuccess, id);

            return book?.Adapt<BookResponse>();
        }

        public async Task<BookResponse> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CreateBookStarted, request.Title);

            var book = request.Adapt<Book>();
            await _repository.AddAsync(book, cancellationToken);
            
            _logger.LogInformation(LoggerMessages.CreateBookSuccess, book.Id, book.Title);

            return book.Adapt<BookResponse>();
        }

        public async Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.DeleteBookStarted, id);

            var book = await _repository.GetByIdAsync(id, cancellationToken);
            if (book == null)
            {
                _logger.LogWarning(LoggerMessages.DeleteBookNotFound, id);
                throw new BookNotFoundException(id);
            }

            await _repository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation(LoggerMessages.DeleteBookSuccess, id);

            return true;
        }

        public async Task<bool> UpdateBookAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.UpdateBookStarted, id);

            var existing = await _repository.GetByIdAsync(id, cancellationToken);

            if (existing == null)
            {
                _logger.LogWarning(LoggerMessages.UpdateBookNotFound, id);
                throw new BookNotFoundException(id);
            }

            request.Adapt(existing);
            await _repository.UpdateAsync(existing, cancellationToken);

            _logger.LogInformation(LoggerMessages.UpdateBookSuccess, id);

            return true;
        }

        public async Task<decimal?> GetBookPriceAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetBookPriceStarted, id);

            var book = await _repository.GetByIdAsync(id, cancellationToken);

            if (book == null)
            {
                _logger.LogWarning(LoggerMessages.GetBookPriceNotFound, id);
                throw new BookNotFoundException(id);
            }

            _logger.LogInformation(LoggerMessages.GetBookPriceSuccess, id, book.Price);

            return book?.Price;
        }
    }
}
