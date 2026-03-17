using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PS.OrderService.Application.DTOs;
using PS.OrderService.Infrastructure.Clients;
using System.Net;

namespace PS.OrderService.UnitTests.Infrastructure.Clients
{
    public class PaymentServiceClientTests
    {
        private readonly Mock<ILogger<PaymentServiceClient>> _loggerMock = new();

        private PaymentServiceClient CreateClient(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            return new PaymentServiceClient(httpClient, _loggerMock.Object);
        }

        [Fact]
        public async Task CreatePaymentAsync_ShouldSucceed_WhenResponseIsSuccess()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            var client = CreateClient(response);

            var request = new CreatePaymentRequest
            {
                OrderId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Amount = 100m
            };

            // Act
            await client.CreatePaymentAsync(request, default);

            // Assert
            // если исключения нет — тест успешен
        }
    }
}
