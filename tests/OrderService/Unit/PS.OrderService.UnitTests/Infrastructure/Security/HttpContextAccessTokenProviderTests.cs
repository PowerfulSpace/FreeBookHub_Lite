using Microsoft.AspNetCore.Http;
using Moq;
using PS.OrderService.Infrastructure.Security;

namespace PS.OrderService.UnitTests.Infrastructure.Security
{
    public class HttpContextAccessTokenProviderTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly HttpContextAccessTokenProvider _provider;

        public HttpContextAccessTokenProviderTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _provider = new HttpContextAccessTokenProvider(_httpContextAccessorMock.Object);
        }

        [Fact]
        public void GetAccessToken_ShouldReturnToken_WhenAuthorizationHeaderExists()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer test-token";

            _httpContextAccessorMock
                .Setup(x => x.HttpContext)
                .Returns(context);

            var result = _provider.GetAccessToken();

            Assert.Equal("Bearer test-token", result);
        }

    }
}
