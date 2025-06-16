using PS.FreeBookHub_Lite.OrderService.Common;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Order;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.Payment;
using PS.FreeBookHub_Lite.OrderService.Domain.Exceptions.User;

namespace PS.FreeBookHub_Lite.OrderService.API.Middleware
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
                    case OrderNotFoundException orderEx:
                        HandleOrderNotFound(context, orderEx);
                        break;

                    case PaymentFailedException paymentEx:
                        HandlePaymentFailed(context, paymentEx);
                        break;

                    case InvalidOrderOperationException invalidOpEx:
                        HandleInvalidOperation(context, invalidOpEx);
                        break;

                    case CannotCancelOrderException cancelEx:
                        HandleCannotCancelOrder(context, cancelEx);
                        break;

                    case InvalidOrderPaymentStateException paymentStateEx:
                        HandleInvalidPaymentState(context, paymentStateEx);
                        break;

                    case InvalidOrderQuantityException quantityEx:
                        HandleInvalidQuantity(context, quantityEx);
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
        private void HandleOrderNotFound(HttpContext context, OrderNotFoundException ex)
        {
            _logger.LogWarning(LoggerMessages.OrderNotFound, ex.OrderId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandlePaymentFailed(HttpContext context, PaymentFailedException ex)
        {
            _logger.LogError(LoggerMessages.PaymentFailedLog, ex.OrderId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired; // Или 400
        }

        private void HandleInvalidOperation(HttpContext context, InvalidOrderOperationException ex)
        {
            _logger.LogWarning(LoggerMessages.InvalidOrderOperation, ex.Message, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleCannotCancelOrder(HttpContext context, CannotCancelOrderException ex)
        {
            _logger.LogWarning(LoggerMessages.CannotCancelOrder, ex.OrderId, ex.CurrentStatus, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleInvalidPaymentState(HttpContext context, InvalidOrderPaymentStateException ex)
        {
            _logger.LogWarning(LoggerMessages.InvalidPaymentState, ex.OrderId, ex.CurrentStatus, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleInvalidQuantity(HttpContext context, InvalidOrderQuantityException ex)
        {
            _logger.LogWarning(LoggerMessages.InvalidQuantity, ex.ProvidedQuantity, context.Request.Method, context.Request.Path);
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
            _logger.LogError(ex, LoggerMessages.UnhandledException, ex.Message, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}