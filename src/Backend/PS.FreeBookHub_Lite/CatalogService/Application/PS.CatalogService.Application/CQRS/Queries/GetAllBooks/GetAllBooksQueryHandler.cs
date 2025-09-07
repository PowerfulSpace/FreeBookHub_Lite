using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.CatalogService.Application.DTOs;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Common.Logging;

namespace PS.CatalogService.Application.CQRS.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookResponse>>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<GetAllBooksQueryHandler> _logger;

        public GetAllBooksQueryHandler(IBookRepository repository, ILogger<GetAllBooksQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<BookResponse>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetAllBooksStarted);

            var books = await _repository.GetAllAsync(cancellationToken);

            var response = books.Adapt<IEnumerable<BookResponse>>();

            _logger.LogInformation(LoggerMessages.GetAllBooksSuccess, response.Count());

            return response;
        }
    }
}
