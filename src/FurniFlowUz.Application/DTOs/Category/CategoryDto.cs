namespace FurniFlowUz.Application.DTOs.Category;

/// <summary>
/// Category details DTO
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Unique category identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Retail price
    /// </summary>
    public decimal RetailPrice { get; set; }

    /// <summary>
    /// Wholesale price
    /// </summary>
    public decimal WholesalePrice { get; set; }

    /// <summary>
    /// Minimum advance payment percentage
    /// </summary>
    public decimal MinAdvancePercent { get; set; }

    /// <summary>
    /// Standard production days for this category
    /// </summary>
    public int StandardProductionDays { get; set; }

    /// <summary>
    /// Indicates if the category is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date and time when category was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when category was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
