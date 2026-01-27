namespace FurniFlowUz.Application.DTOs.TaskPerformance;

/// <summary>
/// Task Performance DTO
/// </summary>
public class TaskPerformanceDto
{
    /// <summary>
    /// Performance record ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Detail task ID
    /// </summary>
    public int DetailTaskId { get; set; }

    /// <summary>
    /// Actual duration taken to complete the task
    /// </summary>
    public TimeSpan ActualDuration { get; set; }

    /// <summary>
    /// Efficiency percentage (Estimated/Actual * 100)
    /// </summary>
    public decimal EfficiencyPercent { get; set; }

    /// <summary>
    /// Quality score (1-10)
    /// </summary>
    public int QualityScore { get; set; }

    /// <summary>
    /// Whether task required rework
    /// </summary>
    public bool RequiredRework { get; set; }

    /// <summary>
    /// Reason for rework if applicable
    /// </summary>
    public string? ReworkReason { get; set; }

    /// <summary>
    /// Date and time when performance was recorded
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
