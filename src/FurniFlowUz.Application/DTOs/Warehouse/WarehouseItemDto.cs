namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// Warehouse item details DTO
/// </summary>
public class WarehouseItemDto
{
    /// <summary>
    /// Unique item identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Item name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Stock Keeping Unit (SKU)
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Current stock quantity
    /// </summary>
    public decimal CurrentStock { get; set; }

    /// <summary>
    /// Minimum stock level (alert threshold)
    /// </summary>
    public decimal MinimumStock { get; set; }

    /// <summary>
    /// Unit of measurement (e.g., pcs, kg, m)
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Primary supplier
    /// </summary>
    public string? Supplier { get; set; }

    /// <summary>
    /// Item description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if item is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates if stock is below minimum level
    /// </summary>
    public bool IsLowStock => CurrentStock <= MinimumStock;

    /// <summary>
    /// Date and time when item was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when item was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
