namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// Warehouse alert DTO for low stock notifications
/// </summary>
public class WarehouseAlertDto
{
    /// <summary>
    /// Warehouse item identifier
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// Warehouse item identifier (alias for ItemId)
    /// </summary>
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Item name
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Stock Keeping Unit
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Current stock quantity
    /// </summary>
    public decimal CurrentStock { get; set; }

    /// <summary>
    /// Minimum stock level
    /// </summary>
    public decimal MinimumStock { get; set; }

    /// <summary>
    /// Amount needed to reach minimum stock
    /// </summary>
    public decimal Shortage { get; set; }

    /// <summary>
    /// Unit of measurement
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Alert severity (Critical if stock is very low, Warning if approaching minimum)
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Alert message describing the issue
    /// </summary>
    public string AlertMessage { get; set; } = string.Empty;

    /// <summary>
    /// Percentage of stock remaining relative to minimum
    /// </summary>
    public decimal StockPercentage => MinimumStock > 0
        ? Math.Round((CurrentStock / MinimumStock) * 100, 2)
        : 0;
}
