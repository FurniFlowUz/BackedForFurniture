using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.DetailTask;

/// <summary>
/// Simplified DTO for task lists
/// </summary>
public class DetailTaskListDto
{
    /// <summary>
    /// Task ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Detail name
    /// </summary>
    public string DetailName { get; set; } = string.Empty;

    /// <summary>
    /// Assigned employee name
    /// </summary>
    public string AssignedEmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Task sequence
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Task description
    /// </summary>
    public string TaskDescription { get; set; } = string.Empty;

    /// <summary>
    /// Task status
    /// </summary>
    public DetailTaskStatus Status { get; set; }

    /// <summary>
    /// Estimated duration
    /// </summary>
    public TimeSpan? EstimatedDuration { get; set; }

    /// <summary>
    /// Is task currently locked (waiting for dependencies)
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// When task was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
