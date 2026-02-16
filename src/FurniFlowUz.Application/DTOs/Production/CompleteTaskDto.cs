using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// DTO for completing a task
/// </summary>
public class CompleteTaskDto
{
    /// <summary>
    /// Actual hours spent on the task (optional)
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Actual hours must be 0 or greater")]
    public decimal? ActualHours { get; set; }

    /// <summary>
    /// Notes about task completion
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }
}
