using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// DTO for creating a new work task
/// </summary>
public class CreateWorkTaskDto
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
    /// Associated order identifier
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }

    /// <summary>
    /// Associated furniture type identifier (optional)
    /// </summary>
    public int? FurnitureTypeId { get; set; }

    /// <summary>
    /// Production stage identifier
    /// </summary>
    [Required(ErrorMessage = "Production stage ID is required")]
    public int ProductionStageId { get; set; }

    /// <summary>
    /// Team identifier to assign the task
    /// </summary>
    [Required(ErrorMessage = "Team ID is required")]
    public int TeamId { get; set; }

    /// <summary>
    /// Sequence order for task execution
    /// </summary>
    [Required(ErrorMessage = "Sequence order is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Sequence order must be at least 1")]
    public int SequenceOrder { get; set; }

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
