using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using PS.FreeBookHub_Lite.CartService.Common;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.CartService.Infrastructure.Clients
{
    public class BookCatalogClient : IBookCatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookCatalogClient> _logger;

        public BookCatalogClient(HttpClient httpClient, ILogger<BookCatalogClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal?> GetBookPriceAsync(Guid bookId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.GetBookPriceStarted, bookId);

            var response = await _httpClient.GetAsync($"/api/books/{bookId}/price", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<decimal>(cancellationToken);
            _logger.LogInformation(LoggerMessages.GetBookPriceSuccess, bookId);

            return result;
        }
    }
}
