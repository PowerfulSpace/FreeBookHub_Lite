using Microsoft.AspNetCore.Authorization;

namespace PS.FreeBookHub_Lite.OrderService.API.Security
{
    public class InternalApiKeyHandler : AuthorizationHandler<InternalApiKeyRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public InternalApiKeyHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, InternalApiKeyRequirement requirement)
        {
            var headers = _httpContextAccessor.HttpContext?.Request.Headers;
            var expectedKey = _configuration["InternalApi:SecretKey"];

            if (!string.IsNullOrEmpty(expectedKey) &&
                headers?.TryGetValue("X-Internal-Key", out var actualKey) == true &&
                actualKey == expectedKey)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
