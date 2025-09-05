using Microsoft.AspNetCore.Authorization;

namespace PS.FreeBookHub_Lite.PaymentService.API.Security
{
    public class InternalApiKeyRequirement : IAuthorizationRequirement
    {
    }
}
