using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Common.Logging;
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
            _logger.LogInformation(LoggerMessages.UserRegistrationStarted, request.Email);

            var result = await _authService.RegisterAsync(request, ct);

            _logger.LogInformation(LoggerMessages.UserRegistrationCompleted, request.Email);

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Аутентификация пользователя", Description = "Выполняет вход пользователя и возвращает токены доступа")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            _logger.LogInformation(LoggerMessages.UserLoginAttempt, request.Email);

            var result = await _authService.LoginAsync(request, ct);

            _logger.LogInformation(LoggerMessages.UserLoginSuccess, request.Email);

            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Обновление токена", Description = "Обновляет access token с помощью refresh token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            _logger.LogInformation(LoggerMessages.TokenRefreshAttempt, userId);
            
            var result = await _authService.RefreshTokenAsync(request, ct);

            _logger.LogInformation(LoggerMessages.TokenRefreshSuccess, userId);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из текущей сессии", Description = "Завершает текущую сессию пользователя")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            _logger.LogInformation(LoggerMessages.UserLogoutAttempt, userId);
            
            await _authService.LogoutCurrentSessionAsync(request, ct);

            _logger.LogInformation(LoggerMessages.UserLogoutSuccess, userId);

            return NoContent();
        }

        [HttpPost("logout-all")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из всех сессий", Description = "Завершает все активные сессии пользователя")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();
            _logger.LogInformation(LoggerMessages.UserLogoutAllAttempt, userId);

            await _authService.LogoutAllSessionsAsync(userId, ct);

            _logger.LogInformation(LoggerMessages.UserLogoutAllSuccess, userId);

            return NoContent();
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {
            _logger.LogInformation(LoggerMessages.UserIdExtractionAttempt);

            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst("nameidentifier")?.Value;

            if (!Guid.TryParse(userId, out var result))
            {
                _logger.LogWarning(LoggerMessages.UserIdExtractionFailed, userId);
                throw new UnauthorizedAccessException("Invalid user ID format");
            }

            _logger.LogInformation(LoggerMessages.UserIdExtractionSuccess, result);

            return result;
        }
    }
}
