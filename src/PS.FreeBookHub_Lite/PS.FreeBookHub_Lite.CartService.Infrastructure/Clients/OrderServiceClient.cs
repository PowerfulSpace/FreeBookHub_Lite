using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.CartService.Infrastructure.Clients
{
    public class OrderServiceClient : IOrderServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderServiceClient> _logger;

        public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "/api/orders",
                    request,
                    cancellationToken
                );

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<OrderResponse>(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
