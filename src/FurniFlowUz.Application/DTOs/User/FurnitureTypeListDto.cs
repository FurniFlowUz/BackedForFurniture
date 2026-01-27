namespace FurniFlowUz.Application.DTOs.User;

/// <summary>
/// Simple DTO for furniture type listing (for constructor use)
/// </summary>
public class FurnitureTypeListDto
{
    /// <summary>
    /// Furniture type ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Furniture type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Associated order ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Order number for reference
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Number of details (components) in this furniture type
    /// </summary>
    public int DetailsCount { get; set; }

    /// <summary>
    /// Number of drawings for this furniture type
    /// </summary>
    public int DrawingsCount { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Has technical specification
    /// </summary>
    public bool HasTechnicalSpecification { get; set; }
}
