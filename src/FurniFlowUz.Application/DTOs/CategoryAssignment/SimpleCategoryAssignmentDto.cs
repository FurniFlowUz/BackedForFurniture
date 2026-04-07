using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.CategoryAssignment;

/// <summary>
/// Simplified DTO for creating a category assignment
/// Only requires OrderId, CategoryName, and TeamLeaderId
/// FurnitureTypeId and TeamId are auto-resolved
/// </summary>
public class SimpleCategoryAssignmentDto
{
    /// <summary>
    /// Order ID to assign category from
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }

    /// <summary>
    /// Category name (will be matched to FurnitureType)
    /// </summary>
    [Required(ErrorMessage = "Category name is required")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Team leader user ID to assign to
    /// </summary>
    [Required(ErrorMessage = "Team Leader ID is required")]
    public int TeamLeaderId { get; set; }

    /// <summary>
    /// Optional notes for the assignment
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}
