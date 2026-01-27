using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// DTO for completing a task
/// </summary>
public class CompleteTaskDto
{
    /// <summary>
    /// Task identifier
    /// </summary>
    [Required(ErrorMessage = "Task ID is required")]
    public int? TaskId { get; set; }

    /// <summary>
    /// Actual hours spent on the task
    /// </summary>
    [Required(ErrorMessage = "Actual hours is required")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Actual hours must be greater than 0")]
    public decimal ActualHours { get; set; }

    /// <summary>
    /// Notes about task completion
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }
}
