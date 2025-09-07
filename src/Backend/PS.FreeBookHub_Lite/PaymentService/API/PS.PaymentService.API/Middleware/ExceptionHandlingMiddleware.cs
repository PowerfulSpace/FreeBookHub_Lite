using PS.PaymentService.Common.Logging;
using PS.PaymentService.Domain.Exceptions.Payment;
using PS.PaymentService.Domain.Exceptions.User;

namespace PS.PaymentService.API.Middleware
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
                    case PaymentNotFoundException notFoundEx:
                        HandlePaymentNotFound(context, notFoundEx);
                        break;

                    case UnauthorizedPaymentAccessException unauthorizedEx:
                        HandleUnauthorizedAccess(context, unauthorizedEx);
                        break;

                    case DuplicatePaymentException duplicateEx:
                        HandleDuplicatePayment(context, duplicateEx);
                        break;

                    case InvalidPaymentStatusException statusEx:
                        HandleInvalidPaymentStatus(context, statusEx);
                        break;

                    case InvalidUserIdentifierException userEx:
                        HandleInvalidUserIdentifier(context, userEx);
                        break;

                    default:
                        HandleUnhandledException(context, ex);
                        break;
                }

                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = context.Response.StatusCode,
                        error = ex.Message
                    });
                }
            }
        }

        private void HandlePaymentNotFound(HttpContext context, PaymentNotFoundException ex)
        {
            _logger.LogWarning(LoggerMessages.PaymentNotFound, ex.PaymentId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandleUnauthorizedAccess(HttpContext context, UnauthorizedPaymentAccessException ex)
        {
            _logger.LogWarning(LoggerMessages.UnauthorizedAccess,
                ex.PaymentId, ex.UserId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }

        private void HandleDuplicatePayment(HttpContext context, DuplicatePaymentException ex)
        {
            _logger.LogWarning(LoggerMessages.DuplicatePayment,
                ex.OrderId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status409Conflict;
        }

        private void HandleInvalidPaymentStatus(HttpContext context, InvalidPaymentStatusException ex)
        {
            _logger.LogWarning(LoggerMessages.InvalidPaymentStatus,
                ex.CurrentStatus, ex.RequiredStatus, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleInvalidUserIdentifier(HttpContext context, InvalidUserIdentifierException ex)
        {
            _logger.LogWarning(LoggerMessages.InvalidUserIdentifier,
                ex.InvalidId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized; // 401 - Unauthorized
        }

        private void HandleUnhandledException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, LoggerMessages.UnhandledException,
                ex.Message, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
