using FurniFlowUz.Application.DTOs.Auth;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// Full work task details DTO
/// </summary>
public class WorkTaskDto
{
    /// <summary>
    /// Unique task identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Task title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Task description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Associated order identifier
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Order number for reference
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Associated furniture type identifier (optional)
    /// </summary>
    public int? FurnitureTypeId { get; set; }

    /// <summary>
    /// Furniture type name for reference
    /// </summary>
    public string? FurnitureTypeName { get; set; }

    /// <summary>
    /// Production stage identifier
    /// </summary>
    public int? ProductionStageId { get; set; }

    /// <summary>
    /// Production stage information
    /// </summary>
    public ProductionStageDto ProductionStage { get; set; } = null!;

    /// <summary>
    /// Assigned team
    /// </summary>
    public TeamDto? AssignedTeam { get; set; }

    /// <summary>
    /// Assigned worker (if task is accepted)
    /// </summary>
    public UserDto? AssignedWorker { get; set; }

    /// <summary>
    /// Task status (Pending, Assigned, InProgress, Completed, Cancelled)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Sequence order for task execution
    /// </summary>
    public int SequenceOrder { get; set; }

    /// <summary>
    /// Estimated hours to complete
    /// </summary>
    public decimal EstimatedHours { get; set; }

    /// <summary>
    /// Actual hours spent (filled when completed)
    /// </summary>
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// Date when task was accepted by worker
    /// </summary>
    public DateTime? AcceptedAt { get; set; }

    /// <summary>
    /// Date when task was started
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date when task was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User who created the task
    /// </summary>
    public UserDto? CreatedBy { get; set; }

    /// <summary>
    /// Date and time when task was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when task was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
