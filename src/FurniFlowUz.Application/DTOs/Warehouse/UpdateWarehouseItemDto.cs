using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// DTO for updating an existing warehouse item
/// </summary>
public class UpdateWarehouseItemDto
{
    /// <summary>
    /// Item name
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Stock Keeping Unit (SKU)
    /// </summary>
    [Required(ErrorMessage = "SKU is required")]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Current stock quantity
    /// </summary>
    [Required(ErrorMessage = "Current stock is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Current stock must be non-negative")]
    public decimal CurrentStock { get; set; }

    /// <summary>
    /// Minimum stock level (alert threshold)
    /// </summary>
    [Required(ErrorMessage = "Minimum stock is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum stock must be non-negative")]
    public decimal MinimumStock { get; set; }

    /// <summary>
    /// Unit of measurement (e.g., pcs, kg, m)
    /// </summary>
    [Required(ErrorMessage = "Unit is required")]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Unit price
    /// </summary>
    [Required(ErrorMessage = "Unit price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Unit price must be non-negative")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Primary supplier
    /// </summary>
    [MaxLength(200)]
    public string? Supplier { get; set; }

    /// <summary>
    /// Item description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Whether the item is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
