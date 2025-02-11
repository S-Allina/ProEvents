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
            List<string?> errors = null;

            switch (exception)
            {
                case ProEvent.Services.Core.Exceptions.ValidationException validationException:
                    status = HttpStatusCode.BadRequest;
                    message = "Validation error.";
                    errors = validationException.Errors;
                    break;
                case ArgumentException argumentException:
                    status = HttpStatusCode.BadRequest;
                    message = "Invalid request data.";
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var errorResponse = new
            {
                message = message,
                errors = errors
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}
