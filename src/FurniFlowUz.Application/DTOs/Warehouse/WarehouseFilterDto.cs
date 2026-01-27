using FurniFlowUz.Application.DTOs.Common;

namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// Filter DTO for warehouse item queries
/// </summary>
public class WarehouseFilterDto : BaseFilter
{
    /// <summary>
    /// Filter to show only items with low stock
    /// </summary>
    public bool? IsLowStock { get; set; }
}
