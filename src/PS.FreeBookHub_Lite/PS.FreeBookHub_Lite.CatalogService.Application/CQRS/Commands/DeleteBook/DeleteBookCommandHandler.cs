﻿using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CatalogService.Application.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Common;
using PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<DeleteBookCommandHandler> _logger;

        public DeleteBookCommandHandler(
            IBookRepository repository,
            ILogger<DeleteBookCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.DeleteBookStarted, request.Id);

            var book = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (book == null)
            {
                throw new BookNotFoundException(request.Id);
            }

            await _repository.DeleteAsync(request.Id, cancellationToken);

            _logger.LogInformation(LoggerMessages.DeleteBookSuccess, request.Id);

            return true;
        }
    }
}
