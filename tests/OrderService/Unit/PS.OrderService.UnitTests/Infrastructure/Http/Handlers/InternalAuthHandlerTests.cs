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
            // Arrange
            var client = CreateClient("my-secret", out var capturedRequest);

            // Act
            await client.GetAsync("/test");

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest!.Headers.Contains("X-Internal-Key"));

            var headerValue = capturedRequest.Headers.GetValues("X-Internal-Key").First();
            Assert.Equal("my-secret", headerValue);
        }

        [Fact]
        public async Task SendAsync_ShouldNotAddHeader_WhenSecretKeyIsNull()
        {
            // Arrange
            var client = CreateClient(null, out var capturedRequest);

            // Act
            await client.GetAsync("/test");

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.False(capturedRequest!.Headers.Contains("X-Internal-Key"));
        }

        [Fact]
        public async Task SendAsync_ShouldCallInnerHandler()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["InternalApi:SecretKey"]).Returns("secret");

            var innerHandlerMock = new Mock<HttpMessageHandler>();

            innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var handler = new InternalAuthHandler(configMock.Object)
            {
                InnerHandler = innerHandlerMock.Object
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            // Act
            await client.GetAsync("/test");

            // Assert
            innerHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}

