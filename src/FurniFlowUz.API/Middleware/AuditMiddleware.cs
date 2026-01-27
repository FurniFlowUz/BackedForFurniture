using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Repositories;
using System.Text;
using System.Text.Json;

namespace FurniFlowUz.API.Middleware;

/// <summary>
/// Middleware for auditing all API requests
/// Logs HTTP method, path, user, timestamp, status code, and request/response data
/// </summary>
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        // Skip audit logging for certain paths (e.g., health checks, swagger)
        if (ShouldSkipAuditLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var request = context.Request;
        var startTime = DateTime.UtcNow;

        // Read request body
        string requestBody = string.Empty;
        if (request.ContentLength > 0 &&
            (request.Method == HttpMethods.Post ||
             request.Method == HttpMethods.Put ||
             request.Method == HttpMethods.Delete))
        {
            request.EnableBuffering();
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            // Log the audit entry
            await LogAuditEntry(
                context,
                unitOfWork,
                request.Method,
                request.Path,
                requestBody,
                startTime,
                context.Response.StatusCode);

            // Copy response back to original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task LogAuditEntry(
        HttpContext context,
        IUnitOfWork unitOfWork,
        string method,
        PathString path,
        string requestBody,
        DateTime timestamp,
        int statusCode)
    {
        try
        {
            // Get user information from claims
            var userIdClaim = context.User?.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                // Skip audit logging for unauthenticated requests (like login)
                return;
            }

            // Parse entity information from the request
            var (entityName, entityId) = ParseEntityFromRequest(path, requestBody);

            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = $"{method} {path}",
                EntityName = entityName,
                EntityId = entityId,
                OldValues = null, // Could be populated by comparing before/after states
                NewValues = SanitizeRequestBody(requestBody),
                Timestamp = timestamp,
                IpAddress = GetClientIpAddress(context)
            };

            await unitOfWork.AuditLogs.AddAsync(auditLog);
            await unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Audit: User {UserId} performed {Action} on {EntityName} {EntityId} at {Timestamp} - Status: {StatusCode}",
                userId, auditLog.Action, entityName, entityId, timestamp, statusCode);
        }
        catch (Exception ex)
        {
            // Don't throw - audit logging failures should not break the application
            _logger.LogError(ex, "Failed to create audit log entry");
        }
    }

    private (string EntityName, int EntityId) ParseEntityFromRequest(PathString path, string requestBody)
    {
        var pathParts = path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Extract entity name from path (e.g., /api/contracts/5 -> Contracts)
        string entityName = pathParts.Length > 1 ? pathParts[1] : "Unknown";

        // Try to extract entity ID from path
        int entityId = 0;
        if (pathParts.Length > 2 && int.TryParse(pathParts[2], out var id))
        {
            entityId = id;
        }
        else if (!string.IsNullOrEmpty(requestBody))
        {
            // Try to extract ID from request body
            try
            {
                using var doc = JsonDocument.Parse(requestBody);
                if (doc.RootElement.TryGetProperty("id", out var idProperty))
                {
                    entityId = idProperty.GetInt32();
                }
            }
            catch
            {
                // Ignore JSON parsing errors
            }
        }

        return (entityName, entityId);
    }

    private string SanitizeRequestBody(string requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
        {
            return string.Empty;
        }

        if (requestBody.Length > 3000)
        {
            return requestBody.Substring(0, 3000);
        }

        // Remove sensitive fields like passwords
        try
        {
            using var doc = JsonDocument.Parse(requestBody);
            var root = doc.RootElement;

            // Check if password field exists and redact it
            if (root.ValueKind == JsonValueKind.Object)
            {
                var dict = new Dictionary<string, object>();
                foreach (var property in root.EnumerateObject())
                {
                    if (property.Name.ToLower().Contains("password"))
                    {
                        dict[property.Name] = "***REDACTED***";
                    }
                    else
                    {
                        dict[property.Name] = property.Value.ToString();
                    }
                }
                return JsonSerializer.Serialize(dict);
            }
        }
        catch
        {
            // If JSON parsing fails, return original (truncated)
        }

        return requestBody;
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }
        return ipAddress ?? "Unknown";
    }

    private bool ShouldSkipAuditLogging(PathString path)
    {
        var skipPaths = new[]
        {
            "/swagger",
            "/health",
            "/api/notifications/hub", // SignalR hub
            "/_framework"
        };

        return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Extension method to register the audit middleware
/// </summary>
public static class AuditMiddlewareExtensions
{
    public static IApplicationBuilder UseAuditMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditMiddleware>();
    }
}
