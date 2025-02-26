using FluentValidation;
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
                case ProEvent.Domain.Exceptions.ValidationException validationException:
                    status = HttpStatusCode.BadRequest;
                    message = "Ошибка валидации.";
                    errors = validationException.Errors;
                    break;

                case FluentValidation.ValidationException fluentValidationException:
                    status = HttpStatusCode.BadRequest;
                    message = "Ошибка валидации." + fluentValidationException.Message;
                    errors = fluentValidationException.Errors.Select(e => e.ErrorMessage).ToList();
                    break;
                case ArgumentException argumentException:
                    status = HttpStatusCode.BadRequest;
                    message = argumentException.Message;
                    break;

                case KeyNotFoundException keyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = keyNotFoundException.Message;
                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Произошла непредвиденная ошибка." + exception.Message;
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