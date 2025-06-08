using PS.FreeBookHub_Lite.AuthService.API.Logging;

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
                _logger.LogError(ex,
                   LoggerMessages.UnhandledException,
                   context.Request?.Method,
                   context.Request?.Path);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    InvalidOperationException when ex.Message.Contains("not found") => StatusCodes.Status404NotFound,
                    InvalidOperationException when ex.Message.Contains("already revoked") => StatusCodes.Status409Conflict,
                    InvalidOperationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

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
