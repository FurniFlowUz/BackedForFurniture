using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// DTO for updating an existing work task
/// </summary>
public class UpdateWorkTaskDto
{
    /// <summary>
    /// Task title
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Task description
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Estimated hours to complete
    /// </summary>
    [Required(ErrorMessage = "Estimated hours is required")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Estimated hours must be greater than 0")]
    public decimal EstimatedHours { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}
