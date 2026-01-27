using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace FurniFlowUz.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// Catches all unhandled exceptions and returns appropriate HTTP responses
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Check if response has already started - if so, we can't modify headers or write body
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started, cannot write error response");
            return;
        }

        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                notFound.Message,
                new List<string> { notFound.Message }
            ),
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "Validation failed",
                validation.Errors?.SelectMany(e => e.Value).ToList() ?? new List<string> { validation.Message }
            ),
            UnauthorizedException unauthorized => (
                HttpStatusCode.Unauthorized,
                unauthorized.Message,
                new List<string> { unauthorized.Message }
            ),
            BusinessException business => (
                HttpStatusCode.BadRequest,
                business.Message,
                new List<string> { business.Message }
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "An internal server error occurred",
                _environment.IsDevelopment()
                    ? new List<string> { exception.Message, exception.StackTrace ?? string.Empty }
                    : new List<string> { "An internal server error occurred. Please contact support." }
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.FailureResponse(message, errors);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension method to register the exception middleware
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
