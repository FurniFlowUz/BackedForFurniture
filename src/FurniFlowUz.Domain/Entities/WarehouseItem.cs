using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class WarehouseItem : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentStock { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinimumStock { get; set; }

    [Required]
    public UnitOfMeasurement Unit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<WarehouseTransaction> Transactions { get; set; } = new List<WarehouseTransaction>();
}
