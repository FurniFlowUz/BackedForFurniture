namespace FurniFlowUz.Application.DTOs.Auth;

/// <summary>
/// DTO for login response containing authentication tokens
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration date and time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = null!;
}
