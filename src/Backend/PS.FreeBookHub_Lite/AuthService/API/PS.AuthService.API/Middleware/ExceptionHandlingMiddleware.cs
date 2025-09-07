using PS.AuthService.Common.Logging;
using PS.AuthService.Domain.Exceptions.Role;
using PS.AuthService.Domain.Exceptions.Token;
using PS.AuthService.Domain.Exceptions.User;

namespace PS.AuthService.API.Middleware
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

                if (!context.Response.HasStarted)
                {
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

        private void HandleInvalidUserIdentifier(HttpContext context)
        {
            _logger.LogWarning(LoggerMessages.InvalidUserIdentifier, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleInvalidCredentials(HttpContext context)
        {
            _logger.LogWarning(LoggerMessages.InvalidCredentials, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleDeactivatedUser(HttpContext context, DeactivatedUserException ex)
        {
            _logger.LogWarning(LoggerMessages.DeactivatedUser, ex.UserId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleUserAlreadyExists(HttpContext context, UserAlreadyExistsException ex)
        {
            _logger.LogWarning(LoggerMessages.UserAlreadyExists, ex.Email, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleUserNotFound(HttpContext context, UserByIdNotFoundException ex)
        {
            _logger.LogWarning(LoggerMessages.UserNotFound, ex.UserId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandleTokenNotFound(HttpContext context, TokenNotFoundException ex)
        {
            _logger.LogWarning(LoggerMessages.TokenNotFound, ex.Token, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleInvalidToken(HttpContext context, InvalidTokenException ex)
        {
            _logger.LogWarning(LoggerMessages.InvalidToken, ex.Token, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleRevokedToken(HttpContext context, RevokedTokenException ex)
        {
            _logger.LogWarning(LoggerMessages.RevokedToken, ex.Token, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private void HandleRoleManagementError(HttpContext context)
        {
            _logger.LogWarning(LoggerMessages.RoleManagementError, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleUnhandledException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, LoggerMessages.UnhandledException, ex.Message, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}