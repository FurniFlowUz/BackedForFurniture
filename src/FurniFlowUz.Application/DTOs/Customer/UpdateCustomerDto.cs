using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Customer;

/// <summary>
/// DTO for updating an existing customer
/// </summary>
public class UpdateCustomerDto
{
    /// <summary>
    /// Customer's first name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Company name (optional, for business customers)
    /// </summary>
    [MaxLength(200)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Customer's phone number
    /// </summary>
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100)]
    public string? Email { get; set; }

    /// <summary>
    /// Customer's address
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Tax identification number (for business customers)
    /// </summary>
    [MaxLength(50)]
    public string? TaxNumber { get; set; }

    /// <summary>
    /// Additional notes about the customer
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}
