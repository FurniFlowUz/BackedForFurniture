using FurniFlowUz.Application.DTOs.Auth;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// Team details DTO with members
/// </summary>
public class TeamDto
{
    /// <summary>
    /// Unique team identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Team leader
    /// </summary>
    public UserDto TeamLeader { get; set; } = null!;

    /// <summary>
    /// Team members (workers)
    /// </summary>
    public List<UserDto> Members { get; set; } = new();

    /// <summary>
    /// Indicates if the team is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date and time when team was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when team was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
