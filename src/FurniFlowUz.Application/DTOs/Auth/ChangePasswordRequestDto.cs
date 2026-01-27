using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Auth;

/// <summary>
/// DTO for changing user password
/// </summary>
public class ChangePasswordRequestDto
{
    /// <summary>
    /// Current password for verification
    /// </summary>
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(100)]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password confirmation
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
