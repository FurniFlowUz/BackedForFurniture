namespace FurniFlowUz.Domain.Interfaces;

/// <summary>
/// Service interface for accessing current authenticated user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current authenticated user's ID
    /// </summary>
    /// <returns>User ID if authenticated, null otherwise</returns>
    int? UserId { get; }

    /// <summary>
    /// Gets the current authenticated user's email
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current authenticated user's role
    /// </summary>
    string? Role { get; }

    /// <summary>
    /// Checks if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
