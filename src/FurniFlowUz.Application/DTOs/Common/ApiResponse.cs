namespace FurniFlowUz.Application.DTOs.Common;

/// <summary>
/// Generic API response wrapper for consistent API responses
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message describing the result of the operation
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The actual data payload
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Collection of error messages if the request failed
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    /// <summary>
    /// Creates a failure response with errors
    /// </summary>
    public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? new List<string> { message }
        };
    }
}
