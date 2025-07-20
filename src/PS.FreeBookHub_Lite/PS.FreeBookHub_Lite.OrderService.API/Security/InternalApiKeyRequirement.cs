using Microsoft.AspNetCore.Authorization;

namespace PS.FreeBookHub_Lite.OrderService.API.Security
{
    public class InternalApiKeyRequirement : IAuthorizationRequirement
    {
    }
}