﻿using MediatR;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CatalogService.Application.Interfaces;
using PS.FreeBookHub_Lite.CatalogService.Common;
using PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book;

namespace PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Queries.GetBookPrice
{
    public class GetBookPriceQueryHandler : IRequestHandler<GetBookPriceQuery, decimal?>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<GetBookPriceQueryHandler> _logger;

        public GetBookPriceQueryHandler(IBookRepository repository, ILogger<GetBookPriceQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<decimal?> Handle(GetBookPriceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetBookPriceStarted, request.Id);

            var book = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (book == null)
            {
                throw new BookNotFoundException(request.Id);
            }

            _logger.LogInformation(LoggerMessages.GetBookPriceSuccess, request.Id, book.Price);

            return book.Price;
        }
    }
}
