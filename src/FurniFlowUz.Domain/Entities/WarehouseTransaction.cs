using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class WarehouseTransaction : BaseAuditableEntity
{
    [Required]
    public WarehouseTransactionType Type { get; set; }

    [Required]
    public int WarehouseItemId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    public int? TeamId { get; set; }

    [Required]
    public int CreatedByUserId { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(WarehouseItemId))]
    public WarehouseItem WarehouseItem { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    public Team? Team { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public User CreatedByUser { get; set; } = null!;

    public MaterialRequest? MaterialRequest { get; set; }
}
