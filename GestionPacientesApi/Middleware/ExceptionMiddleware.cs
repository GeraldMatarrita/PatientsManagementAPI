using GestionPacientesApi.Application.DTOs;
using System.Net;
using System.Text.Json;


namespace GestionPacientesApi.Middleware
{
    // Middleware that intercepts exceptions thrown during the request processing pipeline,
    // logs them, and returns a standardized error response to the client.
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {

        // Attributes for the middleware, including the next delegate in the pipeline and a logger for error logging.
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;

        // Constructor for the middleware that takes a RequestDelegate and ILogger as parameters.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        // Handles exceptions by logging them and returning a standardized error response.
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Create a standardized error response object
            var response = new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred."
            };

            // Determine the specific type of exception and set the appropriate status code and message.
            switch (exception)
            {
                case KeyNotFoundException _:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = exception.Message;
                    break;
                case ArgumentException _:
                case InvalidOperationException _:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    break;
                case UnauthorizedAccessException _:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Unauthorized access.";
                    break;
            }

            // Set the response status code and content type, and serialize the error response to JSON.
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;
            var result = JsonSerializer.Serialize(response);
            // Write the serialized error response to the HTTP response body.
            return context.Response.WriteAsync(result);
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}