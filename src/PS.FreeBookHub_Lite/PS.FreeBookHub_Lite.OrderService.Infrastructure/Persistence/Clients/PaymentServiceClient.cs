using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence.Clients
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;

        public PaymentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/payment", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                // логируем
                throw new ApplicationException($"Payment creation failed. Status: {response.StatusCode}, Error: {error}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
