using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Contract;

/// <summary>
/// DTO for creating a new customer inline during contract creation
/// </summary>
public class NewCustomerDto
{
    /// <summary>
    /// Customer's full name (required for quick customer creation)
    /// </summary>
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(200, ErrorMessage = "Full name cannot exceed 200 characters")]
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number (required for identification)
    /// </summary>
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer's address (optional)
    /// </summary>
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// Customer's email address (optional)
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email { get; set; }

    /// <summary>
    /// Additional notes about the customer (optional)
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}
