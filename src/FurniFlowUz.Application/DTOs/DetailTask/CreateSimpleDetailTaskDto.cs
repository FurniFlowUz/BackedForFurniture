using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.DetailTask;

/// <summary>
/// Simplified DTO for creating a detail task from TeamLeader UI
/// </summary>
public class CreateSimpleDetailTaskDto
{
    /// <summary>
    /// Category assignment ID
    /// </summary>
    [Required(ErrorMessage = "Category Assignment ID is required")]
    public int CategoryAssignmentId { get; set; }

    /// <summary>
    /// Task title
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Task description (optional)
    /// </summary>
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Estimated duration in minutes (optional)
    /// </summary>
    public int? EstimatedMinutes { get; set; }

    /// <summary>
    /// Assigned employee ID (optional - can be assigned later)
    /// </summary>
    public int? AssignedEmployeeId { get; set; }
}
