namespace FurniFlowUz.Application.DTOs.Customer;

/// <summary>
/// Customer details DTO
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Unique customer identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Customer's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name (computed from FirstName and LastName)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Company name (optional, for business customers)
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Customer's address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Tax identification number (for business customers)
    /// </summary>
    public string? TaxNumber { get; set; }

    /// <summary>
    /// Additional notes about the customer
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Indicates if the customer is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date and time when customer was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when customer was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
