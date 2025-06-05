using Microsoft.AspNetCore.Http;
using PS.FreeBookHub_Lite.OrderService.Application.Security;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Security
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
