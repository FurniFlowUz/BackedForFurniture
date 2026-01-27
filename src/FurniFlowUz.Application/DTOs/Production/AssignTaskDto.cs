using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// DTO for assigning a task to a worker
/// </summary>
public class AssignTaskDto
{
    /// <summary>
    /// Task identifier
    /// </summary>
    [Required(ErrorMessage = "Task ID is required")]
    public int? TaskId { get; set; }

    /// <summary>
    /// Worker identifier
    /// </summary>
    [Required(ErrorMessage = "Worker ID is required")]
    public int? WorkerId { get; set; }
}
