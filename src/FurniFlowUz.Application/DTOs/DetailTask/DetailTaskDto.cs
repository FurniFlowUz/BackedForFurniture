using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.DetailTask;

/// <summary>
/// Detail Task DTO
/// </summary>
public class DetailTaskDto
{
    /// <summary>
    /// Unique task identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Category assignment ID
    /// </summary>
    public int CategoryAssignmentId { get; set; }

    /// <summary>
    /// Detail (part) ID
    /// </summary>
    public int DetailId { get; set; }

    /// <summary>
    /// Detail name
    /// </summary>
    public string DetailName { get; set; } = string.Empty;

    /// <summary>
    /// Assigned employee user ID
    /// </summary>
    public int AssignedEmployeeId { get; set; }

    /// <summary>
    /// Assigned employee full name
    /// </summary>
    public string AssignedEmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Task sequence number
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// ID of task that must be completed first (nullable)
    /// </summary>
    public int? DependsOnTaskId { get; set; }

    /// <summary>
    /// Name of dependent task
    /// </summary>
    public string? DependsOnTaskName { get; set; }

    /// <summary>
    /// Task description/instructions
    /// </summary>
    public string TaskDescription { get; set; } = string.Empty;

    /// <summary>
    /// Task status
    /// </summary>
    public DetailTaskStatus Status { get; set; }

    /// <summary>
    /// Estimated duration for this task
    /// </summary>
    public TimeSpan? EstimatedDuration { get; set; }

    /// <summary>
    /// When task was started
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When task was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Order information (for context)
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name (for context)
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Furniture type name (for context)
    /// </summary>
    public string FurnitureTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when task was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when task was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
