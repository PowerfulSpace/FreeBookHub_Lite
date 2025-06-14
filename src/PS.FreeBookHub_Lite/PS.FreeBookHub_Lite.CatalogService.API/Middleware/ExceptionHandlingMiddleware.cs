using PS.FreeBookHub_Lite.CatalogService.Common;
using PS.FreeBookHub_Lite.CatalogService.Domain.Exceptions.Book;

namespace PS.FreeBookHub_Lite.CatalogService.API.Middleware
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

                    case BookAlreadyExistsException bookExistsEx:
                        HandleBookAlreadyExists(context, bookExistsEx);
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
            _logger.LogWarning(LoggerMessages.GetBookByIdNotFound, ex.BookId, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private void HandleBookAlreadyExists(HttpContext context, BookAlreadyExistsException ex)
        {
            _logger.LogWarning(LoggerMessages.CreateBookAlreadyExists, ex.Title, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status409Conflict;
        }

        private void HandleUnhandledException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, LoggerMessages.UnhandledException, ex.Message, context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
