namespace FurniFlowUz.Application.DTOs.User;

/// <summary>
/// DTO for Constructor user information (for order assignment)
/// </summary>
public class ConstructorDto
{
    /// <summary>
    /// Employee ID (used for order assignment)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Constructor full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Constructor phone number
    /// </summary>
    public string? Phone { get; set; }
}
