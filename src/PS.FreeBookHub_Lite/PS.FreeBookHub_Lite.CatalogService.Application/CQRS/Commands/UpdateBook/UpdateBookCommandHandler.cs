using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CatalogService.Application.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Common;
using PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<UpdateBookCommandHandler> _logger;

        public UpdateBookCommandHandler(
            IBookRepository repository,
            ILogger<UpdateBookCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.UpdateBookStarted, request.Id);

            var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (existing == null)
            {
                throw new BookNotFoundException(request.Id);
            }

            var book = request.Adapt(existing);
            await _repository.UpdateAsync(book, cancellationToken);

            _logger.LogInformation(LoggerMessages.UpdateBookSuccess, request.Id);

            return true;
        }
    }
}
