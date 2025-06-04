using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.CartService.Infrastructure.Clients
{
    public class BookCatalogClient : IBookCatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookCatalogClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookCatalogClient(HttpClient httpClient, ILogger<BookCatalogClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<decimal?> GetBookPriceAsync(Guid bookId, CancellationToken cancellationToken)
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.GetAsync($"/api/books/{bookId}/price", cancellationToken);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<decimal>(cancellationToken)
                : null;
        }
    }
}
