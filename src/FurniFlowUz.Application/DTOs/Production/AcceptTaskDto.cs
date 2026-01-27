using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// DTO for worker accepting a task
/// </summary>
public class AcceptTaskDto
{
    /// <summary>
    /// Task identifier
    /// </summary>
    [Required(ErrorMessage = "Task ID is required")]
    public int? TaskId { get; set; }

    /// <summary>
    /// Optional notes when accepting the task
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}
