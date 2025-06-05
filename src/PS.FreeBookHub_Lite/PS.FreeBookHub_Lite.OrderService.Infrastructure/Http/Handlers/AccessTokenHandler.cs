using PS.FreeBookHub_Lite.OrderService.Application.Security;
using System.Net.Http.Headers;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Http.Handlers
{
    public class AccessTokenHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        public AccessTokenHandler(IAccessTokenProvider accessTokenProvider)
        {
            _accessTokenProvider = accessTokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _accessTokenProvider.GetAccessToken();

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
