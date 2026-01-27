using System.ComponentModel.DataAnnotations;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.CategoryAssignment;

/// <summary>
/// DTO for updating a category assignment
/// </summary>
public class UpdateCategoryAssignmentDto
{
    /// <summary>
    /// New assignment status
    /// </summary>
    [Required(ErrorMessage = "Status is required")]
    public CategoryAssignmentStatus Status { get; set; }

    /// <summary>
    /// Optional notes for the update
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}
