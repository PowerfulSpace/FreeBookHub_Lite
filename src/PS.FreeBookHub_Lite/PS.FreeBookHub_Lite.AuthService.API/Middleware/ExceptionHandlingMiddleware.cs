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
                    case InvalidUserIdentifierException:
                        HandleInvalidUserIdentifier(context);
                        break;

                    case InvalidCredentialsException:
                        HandleInvalidCredentials(context);
                        break;

                    case DeactivatedUserException deactivatedEx:
                        HandleDeactivatedUser(context, deactivatedEx);
                        break;

                    case UserAlreadyExistsException existsEx:
                        HandleUserAlreadyExists(context, existsEx);
                        break;

                    case UserByIdNotFoundException notFoundEx:
                        HandleUserNotFound(context, notFoundEx);
                        break;

                    case TokenNotFoundException tokenNotFoundEx:
                        HandleTokenNotFound(context, tokenNotFoundEx);
                        break;

                    case InvalidTokenException invalidTokenEx:
                        HandleInvalidToken(context, invalidTokenEx);
                        break;

                    case RevokedTokenException revokedEx:
                        HandleRevokedToken(context, revokedEx);
                        break;

                    case InvalidRolePromotionException:
                    case RoleAssignmentException:
                        HandleRoleManagementError(context);
                        break;

                    default:
                        HandleUnhandledException(context, ex);
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

        private void HandleInvalidUserIdentifier(HttpContext context)
        {
            _logger.LogWarning("Invalid user ID format | Method: {Method} | Path: {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleInvalidCredentials(HttpContext context)
        {
            _logger.LogWarning("Invalid login attempt | Method: {Method} | Path: {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleDeactivatedUser(HttpContext context, DeactivatedUserException ex)
        {
            _logger.LogWarning("Attempt to access deactivated account: {UserId} | Method: {Method} | Path: {Path}", ex.UserId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleUserAlreadyExists(HttpContext context, UserAlreadyExistsException ex)
        {
            _logger.LogWarning("Attempt to register existing email: {Email} | Method: {Method} | Path: {Path}", ex.Email, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleUserNotFound(HttpContext context, UserByIdNotFoundException ex)
        {
            _logger.LogWarning("User not found: {UserId} | Method: {Method} | Path: {Path}", ex.UserId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandleTokenNotFound(HttpContext context, TokenNotFoundException ex)
        {
            _logger.LogWarning("Refresh token not found: {Token} | Method: {Method} | Path: {Path}", ex.Token, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleInvalidToken(HttpContext context, InvalidTokenException ex)
        {
            _logger.LogWarning("Invalid refresh token: {Token} | Method: {Method} | Path: {Path}", ex.Token, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleRevokedToken(HttpContext context, RevokedTokenException ex)
        {
            _logger.LogWarning("Attempt to use revoked token: {Token} | Method: {Method} | Path: {Path}", ex.Token, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleRoleManagementError(HttpContext context)
        {
            _logger.LogWarning("Role management error | Method: {Method} | Path: {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleUnhandledException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message} | Method: {Method} | Path: {Path}", ex.Message, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
