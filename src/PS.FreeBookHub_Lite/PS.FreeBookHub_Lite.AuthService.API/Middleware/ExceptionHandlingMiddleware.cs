using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Role;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.Token;
using PS.FreeBookHub_Lite.AuthService.Domain.Exceptions.User;

namespace PS.FreeBookHub_Lite.AuthService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    // Авторизация
                    case InvalidUserIdentifierException:
                        _logger.LogWarning(
                            "Invalid user ID format | Method: {Method} | Path: {Path}",
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;

                    case InvalidCredentialsException:
                        _logger.LogWarning(
                            "Invalid login attempt | Method: {Method} | Path: {Path}",
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;

                    case DeactivatedUserException deactivatedEx:
                        _logger.LogWarning(
                            "Attempt to access deactivated account: {UserId} | Method: {Method} | Path: {Path}",
                            deactivatedEx.UserId,
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;

                    // Пользователи
                    case UserAlreadyExistsException existsEx:
                        _logger.LogWarning(
                            "Attempt to register existing email: {Email} | Method: {Method} | Path: {Path}",
                            existsEx.Email,
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;

                    case UserByIdNotFoundException notFoundEx:
                        _logger.LogWarning(
                            "User not found: {UserId} | Method: {Method} | Path: {Path}",
                            notFoundEx.UserId,
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        break;

                    // Токены
                    case TokenNotFoundException tokenNotFoundEx:
                        _logger.LogWarning(
                            "Refresh token not found: {Token} | Method: {Method} | Path: {Path}",
                            tokenNotFoundEx.Token,
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;

                    case InvalidTokenException invalidTokenEx:
                        _logger.LogWarning(
                            "Invalid refresh token: {Token} | Method: {Method} | Path: {Path}",
                            invalidTokenEx.Token,
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;

                    case RevokedTokenException revokedEx:
                        _logger.LogWarning(
                            "Attempt to use revoked token: {Token} | Method: {Method} | Path: {Path}",
                            revokedEx.Token,
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;

                    // Роли
                    case InvalidRolePromotionException:
                    case RoleAssignmentException:
                        _logger.LogWarning(
                            "Role management error | Method: {Method} | Path: {Path}",
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;

                    // Необработанные исключения
                    default:
                        _logger.LogError(
                            ex,
                            "Unhandled exception: {Method} {Path}",
                            context.Request.Method,
                            context.Request.Path);
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }

                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = context.Response.StatusCode,
                    error = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
