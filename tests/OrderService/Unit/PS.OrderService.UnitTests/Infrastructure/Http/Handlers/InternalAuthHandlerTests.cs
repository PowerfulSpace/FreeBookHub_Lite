using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using PS.OrderService.Infrastructure.Http.Handlers;
using System.Net;

namespace PS.OrderService.UnitTests.Infrastructure.Http.Handlers
{
    public class InternalAuthHandlerTests
    {
        private HttpClient CreateClient(string? secretKey, out HttpRequestMessage? capturedRequest)
        {
            HttpRequestMessage? localRequest = null;
            capturedRequest = null;

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["InternalApi:SecretKey"]).Returns(secretKey);

            var innerHandlerMock = new Mock<HttpMessageHandler>();

            innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
                {
                    localRequest = req;
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var handler = new InternalAuthHandler(configMock.Object)
            {
                InnerHandler = innerHandlerMock.Object
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            capturedRequest = localRequest;
            return client;
        }

        [Fact]
        public async Task SendAsync_ShouldAddHeader_WhenSecretKeyExists()
        {
            var client = CreateClient("my-secret", out var capturedRequest);

            await client.GetAsync("/test");

            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest!.Headers.Contains("X-Internal-Key"));

            var headerValue = capturedRequest.Headers.GetValues("X-Internal-Key").First();
            Assert.Equal("my-secret", headerValue);
        }

    }
}
