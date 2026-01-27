using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.CategoryAssignment;

/// <summary>
/// Category Assignment details DTO
/// </summary>
public class CategoryAssignmentDto
{
    /// <summary>
    /// Unique assignment identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Order ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Order number for display
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Furniture type (category) ID
    /// </summary>
    public int FurnitureTypeId { get; set; }

    /// <summary>
    /// Furniture type name
    /// </summary>
    public string FurnitureTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Team leader user ID
    /// </summary>
    public int TeamLeaderId { get; set; }

    /// <summary>
    /// Team leader full name
    /// </summary>
    public string TeamLeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Team ID
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// Assignment status
    /// </summary>
    public CategoryAssignmentStatus Status { get; set; }

    /// <summary>
    /// When the category was assigned
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// When work started on this assignment
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When the assignment was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Total number of detail tasks
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of completed tasks
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Assignment notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date and time when assignment was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when assignment was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
