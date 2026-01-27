namespace FurniFlowUz.Application.DTOs.User;

/// <summary>
/// DTO for ProductionManager information
/// </summary>
public class ProductionManagerDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Employee ID (used for order assignment)
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Whether the account is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Account creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
