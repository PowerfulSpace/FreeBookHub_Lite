using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.CartService.Application.Clients;
using PS.FreeBookHub_Lite.CartService.Application.DTOs.Order;
using PS.FreeBookHub_Lite.CartService.Common;
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
            _logger.LogInformation(LoggerMessages.CreateOrderStarted, request.UserId);

            var response = await _httpClient.PostAsJsonAsync("/api/orders", request, cancellationToken);
            response.EnsureSuccessStatusCode(); // Выбросит исключение при ошибке

            var order = await response.Content.ReadFromJsonAsync<OrderResponse>(cancellationToken);
            _logger.LogInformation(LoggerMessages.CreateOrderSuccess, request.UserId, order.Id);

            return order;
        }
    }
}
