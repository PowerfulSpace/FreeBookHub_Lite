using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.CartService.Application.Services
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
            var response = await _httpClient.GetAsync($"/api/books/{bookId}/price", cancellationToken);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<decimal>(cancellationToken)
                : null;
        }
    }
}
