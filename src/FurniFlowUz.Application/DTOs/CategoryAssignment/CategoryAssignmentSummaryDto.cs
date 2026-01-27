using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.CategoryAssignment;

/// <summary>
/// Summary DTO for category assignment (for lists)
/// </summary>
public class CategoryAssignmentSummaryDto
{
    /// <summary>
    /// Assignment ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Order number
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Furniture type name
    /// </summary>
    public string FurnitureTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Team leader name
    /// </summary>
    public string TeamLeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Team name
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// Assignment status
    /// </summary>
    public CategoryAssignmentStatus Status { get; set; }

    /// <summary>
    /// When assigned
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// Task completion progress (e.g., "5/10")
    /// </summary>
    public string TaskProgress { get; set; } = string.Empty;

    /// <summary>
    /// Completion percentage (0-100)
    /// </summary>
    public decimal CompletionPercent { get; set; }
}
