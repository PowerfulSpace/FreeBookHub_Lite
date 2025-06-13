using PS.FreeBookHub_Lite.CartService.Common;
using PS.FreeBookHub_Lite.CartService.Domain.Exceptions.Cart;

namespace PS.FreeBookHub_Lite.CartService.API.Middleware
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
                    case BookNotFoundException bookEx:
                        HandleBookNotFound(context, bookEx);
                        break;

                    case CartNotFoundException cartEx:
                        HandleCartNotFound(context, cartEx);
                        break;

                    case EmptyCartException emptyEx:
                        HandleEmptyCart(context, emptyEx);
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

        private void HandleBookNotFound(HttpContext context, BookNotFoundException ex)
        {
            _logger.LogWarning(LoggerMessages.BookNotFound, ex.BookId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandleCartNotFound(HttpContext context, CartNotFoundException ex)
        {
            _logger.LogWarning(LoggerMessages.CartNotFound, ex.UserId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandleEmptyCart(HttpContext context, EmptyCartException ex)
        {
            _logger.LogWarning(LoggerMessages.EmptyCart, ex.UserId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private void HandleUnhandledException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, LoggerMessages.UnhandledException, ex.Message, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
