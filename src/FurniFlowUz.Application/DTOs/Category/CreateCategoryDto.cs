using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Category;

/// <summary>
/// DTO for creating a new category
/// </summary>
public class CreateCategoryDto
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
    /// Retail price
    /// </summary>
    [Required(ErrorMessage = "Retail price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Retail price must be greater than 0")]
    public decimal RetailPrice { get; set; }

    /// <summary>
    /// Wholesale price
    /// </summary>
    [Required(ErrorMessage = "Wholesale price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Wholesale price must be greater than 0")]
    public decimal WholesalePrice { get; set; }

    /// <summary>
    /// Minimum advance payment percentage
    /// </summary>
    [Required(ErrorMessage = "Minimum advance percent is required")]
    [Range(0, 100, ErrorMessage = "Minimum advance percent must be between 0 and 100")]
    public decimal MinAdvancePercent { get; set; }

    /// <summary>
    /// Standard production days for this category
    /// </summary>
    [Required(ErrorMessage = "Standard production days is required")]
    [Range(1, 365, ErrorMessage = "Standard production days must be between 1 and 365")]
    public int StandardProductionDays { get; set; }

    /// <summary>
    /// Indicates if the category is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
