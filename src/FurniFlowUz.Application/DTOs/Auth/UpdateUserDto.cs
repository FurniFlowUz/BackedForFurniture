using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Auth;

/// <summary>
/// DTO for updating user information
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// User's first name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number
    /// </summary>
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// User's role
    /// </summary>
    [MaxLength(50)]
    public string? Role { get; set; }

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool? IsActive { get; set; }
}
