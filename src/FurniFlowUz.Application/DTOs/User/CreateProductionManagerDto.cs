using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.User;

/// <summary>
/// DTO for creating a new ProductionManager user
/// </summary>
public class CreateProductionManagerDto
{
    /// <summary>
    /// Email address (used for login)
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password (will be hashed)
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;
}
