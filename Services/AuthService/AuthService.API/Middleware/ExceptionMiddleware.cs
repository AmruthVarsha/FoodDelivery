
using AuthService.Application.Exceptions;

namespace AuthService.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var path = context.Request.Path;

            switch (ex)
            {
                case NotFoundException notFound:
                    _logger.LogInformation("Resource not found at {Path}: {Message}", path, notFound.Message);
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(new { error = notFound.Message });
                    break;

                case UnauthorizedException unauthorized:
                    _logger.LogWarning("Unauthorized access attempt at {Path}: {Message}", path, unauthorized.Message);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = unauthorized.Message });
                    break;

                case ForbiddenException forbidden:
                    _logger.LogWarning("Forbidden access attempt at {Path}: {Message}", path, forbidden.Message);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = forbidden.Message });
                    break;

                case BadRequestException badRequest:
                    _logger.LogInformation("Bad request at {Path}: {Message}", path, badRequest.Message);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { error = badRequest.Message });
                    break;

                case ConflictException conflict:
                    _logger.LogWarning("Conflict at {Path}: {Message}", path, conflict.Message);
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    await context.Response.WriteAsJsonAsync(new { error = conflict.Message });
                    break;

                default:
                    _logger.LogError(ex, "Unhandled exception at {Path}", path);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred. Please try again later." });
                    break;
            }
        }
    }
}
