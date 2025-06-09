using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Регистрация нового пользователя", Description = "Создает новую учетную запись пользователя")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
        {
            var result = await _authService.RegisterAsync(request, ct);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Аутентификация пользователя", Description = "Выполняет вход пользователя и возвращает токены доступа")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request, ct);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Обновление токена", Description = "Обновляет access token с помощью refresh token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var result = await _authService.RefreshTokenAsync(request, ct);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из текущей сессии", Description = "Завершает текущую сессию пользователя")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            await _authService.LogoutCurrentSessionAsync(request, ct);

            return NoContent();
        }

        [HttpPost("logout-all")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из всех сессий", Description = "Завершает все активные сессии пользователя")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            await _authService.LogoutAllSessionsAsync(userId, ct);

            return NoContent();
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {
            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst("nameidentifier")?.Value;

            if (!Guid.TryParse(userId, out var result))
            {
                throw new UnauthorizedAccessException("Invalid user ID format");
            }

            return result;
        }
    }
}
