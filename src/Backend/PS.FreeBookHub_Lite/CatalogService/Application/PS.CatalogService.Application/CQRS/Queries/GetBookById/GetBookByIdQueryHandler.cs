using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.CatalogService.Application.DTOs;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Common.Logging;
using PS.CatalogService.Domain.Exceptions.Book;

namespace PS.CatalogService.Application.CQRS.Queries.GetBookById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookResponse>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<GetBookByIdQueryHandler> _logger;

        public GetBookByIdQueryHandler(IBookRepository repository, ILogger<GetBookByIdQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BookResponse> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetBookByIdStarted, request.Id);

            var book = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (book == null)
            {
                throw new BookNotFoundException(request.Id);
            }

            _logger.LogInformation(LoggerMessages.GetBookByIdSuccess, request.Id);

            return book.Adapt<BookResponse>();
        }
    }
}
