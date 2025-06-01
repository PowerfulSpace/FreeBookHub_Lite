using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PS.FreeBookHub_Lite.AuthService.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public() => Ok("No auth");

        [HttpGet("private")]
        [Authorize]
        public IActionResult Private() => Ok("Auth required");
    }
}
