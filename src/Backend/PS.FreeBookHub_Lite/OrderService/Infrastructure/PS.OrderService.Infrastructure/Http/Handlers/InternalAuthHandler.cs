using Microsoft.Extensions.Configuration;

namespace PS.OrderService.Infrastructure.Http.Handlers
{
    public class InternalAuthHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;

        public InternalAuthHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var internalKey = _configuration["InternalApi:SecretKey"];
            if (!string.IsNullOrEmpty(internalKey))
            {
                request.Headers.Add("X-Internal-Key", internalKey);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
