using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class MaterialRequest : BaseAuditableEntity
{
    [Required]
    public int WarehouseTransactionId { get; set; }

    [Required]
    public int TeamId { get; set; }

    [Required]
    public int RequestedByUserId { get; set; }

    [Required]
    public ConfirmationStatus ConfirmationStatus { get; set; } = ConfirmationStatus.Pending;

    public int? ConfirmedByUserId { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(WarehouseTransactionId))]
    public WarehouseTransaction WarehouseTransaction { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    public Team Team { get; set; } = null!;

    [ForeignKey(nameof(RequestedByUserId))]
    public User RequestedByUser { get; set; } = null!;

    [ForeignKey(nameof(ConfirmedByUserId))]
    public User? ConfirmedByUser { get; set; }
}
