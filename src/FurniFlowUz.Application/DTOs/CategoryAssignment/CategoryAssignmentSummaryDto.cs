using FurniFlowUz.Application.DTOs.Constructor;

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
    /// Furniture type ID
    /// </summary>
    public int FurnitureTypeId { get; set; }

    /// <summary>
    /// Order ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Category name (from OrderCategory)
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Team leader ID
    /// </summary>
    public int TeamLeaderId { get; set; }

    /// <summary>
    /// Team leader name
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
    /// Assignment status (string: Assigned, InProgress, Completed, OnHold)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// When assigned
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// When work started
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Task completion progress (e.g., "5/10")
    /// </summary>
    public string TaskProgress { get; set; } = string.Empty;

    /// <summary>
    /// Completion percentage (0-100)
    /// </summary>
    public decimal CompletionPercent { get; set; }

    /// <summary>
    /// Constructor's Details (components) for this FurnitureType
    /// Team Leader needs this to create tasks based on these details
    /// </summary>
    public List<DetailDto> Details { get; set; } = new();

    /// <summary>
    /// Count of details for quick reference
    /// </summary>
    public int DetailsCount { get; set; }
}
