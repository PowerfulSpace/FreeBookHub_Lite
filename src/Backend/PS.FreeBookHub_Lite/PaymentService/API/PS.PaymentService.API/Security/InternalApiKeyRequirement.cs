using Microsoft.AspNetCore.Authorization;

namespace PS.PaymentService.API.Security
{
    public class InternalApiKeyRequirement : IAuthorizationRequirement
    {
    }
}
