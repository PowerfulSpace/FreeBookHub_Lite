using Microsoft.Extensions.Logging;
using PS.FreeBookHub_Lite.OrderService.Application.Clients;
using PS.FreeBookHub_Lite.OrderService.Application.DTOs;
using PS.FreeBookHub_Lite.OrderService.Common;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Payment;
using System.Net.Http.Json;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Clients
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentServiceClient> _logger;

        public PaymentServiceClient(HttpClient httpClient, ILogger<PaymentServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggerMessages.PaymentCreationStarted, request.OrderId);

            var response = await _httpClient.PostAsJsonAsync("/api/payment", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);

                throw new PaymentFailedException(request.OrderId, (int)response.StatusCode, error);
            }

            _logger.LogInformation(LoggerMessages.PaymentCreationSuccess, request.OrderId);
        }
    }
}
