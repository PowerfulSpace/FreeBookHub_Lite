using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PS.AuthService.Application.CQRS.Commands.Login;
using PS.AuthService.Application.CQRS.Commands.Logout;
using PS.AuthService.Application.CQRS.Commands.LogoutAll;
using PS.AuthService.Application.CQRS.Commands.RefreshToken;
using PS.AuthService.Application.CQRS.Commands.Register;
using PS.AuthService.Application.DTOs;
using PS.AuthService.Domain.Exceptions.User;
using Swashbuckle.AspNetCore.Annotations;

namespace PS.AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Регистрация нового пользователя", Description = "Создает новую учетную запись пользователя")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
        {
            var command = request.Adapt<RegisterCommand>();
            var result = await _mediator.Send(command, ct);

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Аутентификация пользователя", Description = "Выполняет вход пользователя и возвращает токены доступа")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var command = request.Adapt<LoginCommand>();
            var result = await _mediator.Send(command, ct);

            return Ok(result);
        }

        [HttpPost("refresh")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Обновление токена", Description = "Обновляет access token с помощью refresh token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = request.Adapt<RefreshTokenCommand>();
            var result = await _mediator.Send(command, ct);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из текущей сессии", Description = "Завершает текущую сессию пользователя")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = request.Adapt<LogoutCommand>();
            await _mediator.Send(command, ct);

            return NoContent();
        }

        [HttpPost("logout-all")]
        [Authorize(Policy = "User")]
        [SwaggerOperation(Summary = "Выход из всех сессий", Description = "Завершает все активные сессии пользователя")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var userId = GetUserIdFromClaimsOrThrow();

            var command = new LogoutAllCommand() { UserId = userId };
            await _mediator.Send(command, ct);

            return NoContent();
        }

        private Guid GetUserIdFromClaimsOrThrow()
        {
            var userId = User.FindFirst("sub")?.Value
              ?? User.FindFirst("nameidentifier")?.Value;

            if (!Guid.TryParse(userId, out var result))
            {
                throw new InvalidUserIdentifierException(userId ?? "null");
            }

            return result;
        }
    }
}