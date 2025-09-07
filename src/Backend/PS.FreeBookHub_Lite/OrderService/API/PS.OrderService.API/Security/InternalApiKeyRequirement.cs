using Microsoft.AspNetCore.Authorization;

namespace PS.OrderService.API.Security
{
    public class InternalApiKeyRequirement : IAuthorizationRequirement
    {
    }
}