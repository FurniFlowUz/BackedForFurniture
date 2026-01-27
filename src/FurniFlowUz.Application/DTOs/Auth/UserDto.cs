namespace FurniFlowUz.Application.DTOs.Auth;

/// <summary>
/// DTO representing user information
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique user identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name (computed from FirstName and LastName)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the user account is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date and time when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
