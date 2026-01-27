using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.DetailTask;

/// <summary>
/// DTO for creating a new detail task
/// </summary>
public class CreateDetailTaskDto
{
    /// <summary>
    /// Category assignment ID
    /// </summary>
    [Required(ErrorMessage = "Category Assignment ID is required")]
    public int CategoryAssignmentId { get; set; }

    /// <summary>
    /// Detail (part) ID
    /// </summary>
    [Required(ErrorMessage = "Detail ID is required")]
    public int DetailId { get; set; }

    /// <summary>
    /// Assigned employee user ID
    /// </summary>
    [Required(ErrorMessage = "Assigned Employee ID is required")]
    public int AssignedEmployeeId { get; set; }

    /// <summary>
    /// Task sequence number (1, 2, 3, etc.)
    /// </summary>
    [Required(ErrorMessage = "Sequence is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Sequence must be greater than 0")]
    public int Sequence { get; set; }

    /// <summary>
    /// ID of task that must be completed first (optional)
    /// </summary>
    public int? DependsOnTaskId { get; set; }

    /// <summary>
    /// Task description/instructions
    /// </summary>
    [Required(ErrorMessage = "Task description is required")]
    [MaxLength(500, ErrorMessage = "Task description cannot exceed 500 characters")]
    public string TaskDescription { get; set; } = string.Empty;

    /// <summary>
    /// Estimated duration for this task
    /// </summary>
    public TimeSpan? EstimatedDuration { get; set; }
}
