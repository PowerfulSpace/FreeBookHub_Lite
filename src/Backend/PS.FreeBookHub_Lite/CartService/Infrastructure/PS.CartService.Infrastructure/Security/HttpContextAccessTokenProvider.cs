using Microsoft.AspNetCore.Http;
using PS.CartService.Application.Security;

namespace PS.CartService.Infrastructure.Security
{
    public class HttpContextAccessTokenProvider : IAccessTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetAccessToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        }
    }
}