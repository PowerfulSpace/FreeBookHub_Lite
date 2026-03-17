using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PS.OrderService.Infrastructure.Clients;

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
    }
}
