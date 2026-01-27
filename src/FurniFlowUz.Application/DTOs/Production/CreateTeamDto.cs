using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// DTO for creating a new team
/// </summary>
public class CreateTeamDto
{
    /// <summary>
    /// Team name
    /// </summary>
    [Required(ErrorMessage = "Team name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Team leader identifier
    /// </summary>
    [Required(ErrorMessage = "Team leader is required")]
    public int TeamLeaderId { get; set; }

    /// <summary>
    /// Team member identifiers (workers)
    /// </summary>
    public List<int> MemberIds { get; set; } = new();
}
