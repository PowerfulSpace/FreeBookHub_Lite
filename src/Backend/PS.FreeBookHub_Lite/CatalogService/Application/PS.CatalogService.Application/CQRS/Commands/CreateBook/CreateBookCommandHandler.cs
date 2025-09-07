using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.CatalogService.Application.DTOs;
using PS.CatalogService.Application.Interfaces;
using PS.CatalogService.Common.Logging;
using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.Application.CQRS.Commands.CreateBook
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookResponse>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<CreateBookCommandHandler> _logger;

        public CreateBookCommandHandler(IBookRepository repository, ILogger<CreateBookCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BookResponse> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.CreateBookStarted, request.Title);

            var book = request.Adapt<Book>();
            await _repository.AddAsync(book, cancellationToken);

            _logger.LogInformation(LoggerMessages.CreateBookSuccess, book.Id, book.Title);

            return book.Adapt<BookResponse>();
        }
    }
}
