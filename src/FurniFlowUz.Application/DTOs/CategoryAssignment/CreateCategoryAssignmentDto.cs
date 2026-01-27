using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.CategoryAssignment;

/// <summary>
/// DTO for creating a new category assignment
/// </summary>
public class CreateCategoryAssignmentDto
{
    /// <summary>
    /// Order ID to assign category from
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }

    /// <summary>
    /// Furniture type (category) ID to assign
    /// </summary>
    [Required(ErrorMessage = "Furniture Type ID is required")]
    public int FurnitureTypeId { get; set; }

    /// <summary>
    /// Team leader user ID to assign to
    /// </summary>
    [Required(ErrorMessage = "Team Leader ID is required")]
    public int TeamLeaderId { get; set; }

    /// <summary>
    /// Team ID for this assignment
    /// </summary>
    [Required(ErrorMessage = "Team ID is required")]
    public int TeamId { get; set; }

    /// <summary>
    /// Optional notes for the assignment
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}
