using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.TaskPerformance;

/// <summary>
/// DTO for creating task performance record
/// </summary>
public class CreateTaskPerformanceDto
{
    /// <summary>
    /// Detail task ID
    /// </summary>
    [Required(ErrorMessage = "Detail Task ID is required")]
    public int DetailTaskId { get; set; }

    /// <summary>
    /// Actual duration taken to complete the task
    /// </summary>
    [Required(ErrorMessage = "Actual duration is required")]
    public TimeSpan ActualDuration { get; set; }

    /// <summary>
    /// Quality score (1-10)
    /// </summary>
    [Required(ErrorMessage = "Quality score is required")]
    [Range(1, 10, ErrorMessage = "Quality score must be between 1 and 10")]
    public int QualityScore { get; set; }

    /// <summary>
    /// Whether task required rework
    /// </summary>
    public bool RequiredRework { get; set; }

    /// <summary>
    /// Reason for rework if applicable
    /// </summary>
    [MaxLength(500, ErrorMessage = "Rework reason cannot exceed 500 characters")]
    public string? ReworkReason { get; set; }
}
