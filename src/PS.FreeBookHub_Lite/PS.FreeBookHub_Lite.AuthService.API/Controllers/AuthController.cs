using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using System.Security.Claims;

namespace PS.FreeBookHub_Lite.AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthBookService _authService;

        public AuthController(IAuthBookService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
        {
            var result = await _authService.RegisterAsync(request, ct);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request, ct);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await _authService.RefreshTokenAsync(request, ct);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        {
            await _authService.LogoutCurrentSessionAsync(request, ct);
            return NoContent();
        }

        [HttpPost("logout-all")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            await _authService.LogoutAllSessionsAsync(userId, ct);
            return NoContent();
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {

            //var userId = User.FindFirst("sub")?.Value
            // ?? throw new UnauthorizedAccessException("'sub' claim not found");

            //if (!Guid.TryParse(userId, out var result))
            //    throw new UnauthorizedAccessException("Invalid user ID format");

            //return result;

            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var result))
                throw new UnauthorizedAccessException("Invalid user ID format");

            return result;

            //// Ищем по всем возможным вариантам claim'ов для ID
            //var userIdClaim = User.FindFirst(claim =>
            //    claim.Type == "sub" ||
            //    claim.Type == ClaimTypes.NameIdentifier || // Соответствует nameidentifier
            //    claim.Type == "id")?.Value;

            //if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            //{
            //    var availableClaims = User.Claims.Select(c => $"{c.Type}={c.Value}");
            //    throw new UnauthorizedAccessException(
            //        $"User ID claim not found. Available claims: {string.Join(", ", availableClaims)}");
            //}

            //return userId;
        }
    }
}
