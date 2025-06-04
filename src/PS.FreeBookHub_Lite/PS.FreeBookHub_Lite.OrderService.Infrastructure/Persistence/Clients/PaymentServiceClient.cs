using Microsoft.AspNetCore.Http;
using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence.Clients
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

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
