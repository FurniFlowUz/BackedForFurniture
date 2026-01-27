namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// Detail (component) information DTO
/// </summary>
public class DetailDto
{
    /// <summary>
    /// Unique detail identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Detail name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Width in millimeters
    /// </summary>
    public decimal Width { get; set; }

    /// <summary>
    /// Height in millimeters
    /// </summary>
    public decimal Height { get; set; }

    /// <summary>
    /// Thickness in millimeters
    /// </summary>
    public decimal Thickness { get; set; }

    /// <summary>
    /// Quantity needed
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Associated furniture type identifier
    /// </summary>
    public int? FurnitureTypeId { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date and time when detail was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when detail was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
