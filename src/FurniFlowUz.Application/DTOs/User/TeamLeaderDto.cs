namespace FurniFlowUz.Application.DTOs.User;

/// <summary>
/// DTO for Team Leader user information (for category assignment)
/// </summary>
public class TeamLeaderDto
{
    /// <summary>
    /// User ID (used for assignment)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Team Leader full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Team Leader phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Department name
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Position name
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Number of active assignments (InProgress + Assigned)
    /// </summary>
    public int ActiveAssignments { get; set; }

    /// <summary>
    /// Number of completed assignments
    /// </summary>
    public int CompletedAssignments { get; set; }

    /// <summary>
    /// Team name (if any)
    /// </summary>
    public string? TeamName { get; set; }
}
