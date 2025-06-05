using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Clients
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;

        public PaymentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/payment", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                // логируем
                throw new ApplicationException($"Payment creation failed. Status: {response.StatusCode}, Error: {error}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
