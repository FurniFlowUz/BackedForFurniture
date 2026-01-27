using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Category;

/// <summary>
/// DTO for updating an existing category
/// </summary>
public class UpdateCategoryDto
{
    /// <summary>
    /// Category name
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Standard preparation days for this category
    /// </summary>
    [Required(ErrorMessage = "Standard preparation days is required")]
    [Range(1, 365, ErrorMessage = "Standard preparation days must be between 1 and 365")]
    public int StandardPreparationDays { get; set; }

    /// <summary>
    /// Indicates if the category is active
    /// </summary>
    public bool IsActive { get; set; }
}
