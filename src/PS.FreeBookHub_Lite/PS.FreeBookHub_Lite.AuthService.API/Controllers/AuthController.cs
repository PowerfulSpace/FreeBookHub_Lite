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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthBookService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Регистрация нового пользователя", Description = "Создает новую учетную запись пользователя")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
        {
            _logger.LogInformation("Попытка регистрации пользователя с Email: {Email}", request.Email);

            var result = await _authService.RegisterAsync(request, ct);

            _logger.LogInformation("Пользователь с Email {Email} успешно зарегистрирован", request.Email);

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Аутентификация пользователя", Description = "Выполняет вход пользователя и возвращает токены доступа")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            _logger.LogInformation("Попытка входа пользователя с Email: {Email}", request.Email);

            var result = await _authService.LoginAsync(request, ct);

            _logger.LogInformation("Пользователь с Email {Email} успешно вошёл", request.Email);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Обновление токена", Description = "Обновляет access token с помощью refresh token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            _logger.LogInformation("Попытка обновления токена");

            var result = await _authService.RefreshTokenAsync(request, ct);

            _logger.LogInformation("Токен успешно обновлён для пользователя");
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из текущей сессии", Description = "Завершает текущую сессию пользователя")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            _logger.LogInformation("Пользователь {UserId} выходит из текущей сессии", userId);

            await _authService.LogoutCurrentSessionAsync(request, ct);

            _logger.LogInformation("Пользователь {UserId} успешно вышел", userId);
            return NoContent();
        }

        [HttpPost("logout-all")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из всех сессий", Description = "Завершает все активные сессии пользователя")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            _logger.LogInformation("Пользователь {UserId} выходит из всех сессий", userId);

            await _authService.LogoutAllSessionsAsync(userId, ct);

            _logger.LogInformation("Пользователь {UserId} успешно вышел из всех сессий", userId);
            return NoContent();
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {
            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst("nameidentifier")?.Value;

            //var userId = User.FindFirst("sub")?.Value
            // ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var result))
            {
                _logger.LogWarning("Некорректный формат userId в claims: {ClaimValue}", userId);

                throw new UnauthorizedAccessException("Invalid user ID format");
            }
                

            return result;
        }
    }
}
