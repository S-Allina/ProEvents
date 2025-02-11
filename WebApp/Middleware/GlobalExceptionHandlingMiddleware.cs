using System.Net;
using System.Text.Json;

namespace WebApp.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ArgumentException argumentException:
                    status = HttpStatusCode.BadRequest;
                    message = "Invalid request data."; // Общее сообщение для клиента
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = exception.Message; // Общее сообщение для клиента
                    break;
            }

            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message); // Логируем полное исключение

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var errorResponse = new
            {
                message = message,
                // Дополнительные детали, которые можно вернуть (например, requestId)
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}
