namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// Full furniture type details DTO
/// </summary>
public class FurnitureTypeDto
{
    /// <summary>
    /// Unique furniture type identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Furniture type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Associated order identifier
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Order number for reference
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// OrderCategory identifier - links to specific order-category relationship
    /// </summary>
    public int? OrderCategoryId { get; set; }

    /// <summary>
    /// Category identifier (from OrderCategory)
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Category name (from OrderCategory.Category)
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Details (components) for this furniture type
    /// </summary>
    public List<DetailDto> Details { get; set; } = new();

    /// <summary>
    /// Drawings for this furniture type
    /// </summary>
    public List<DrawingDto> Drawings { get; set; } = new();

    /// <summary>
    /// Technical specification (if exists)
    /// </summary>
    public TechnicalSpecificationDto? TechnicalSpecification { get; set; }

    /// <summary>
    /// Date and time when furniture type was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when furniture type was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
