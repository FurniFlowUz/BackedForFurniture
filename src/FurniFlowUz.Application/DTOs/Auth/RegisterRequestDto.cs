using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Auth;

/// <summary>
/// DTO for user registration request
/// </summary>
public class RegisterRequestDto
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
    /// User's email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number
    /// </summary>
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// User role (Director, ProductionManager, Constructor, Worker, Warehouseman)
    /// </summary>
    [Required(ErrorMessage = "Role is required")]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;
}
