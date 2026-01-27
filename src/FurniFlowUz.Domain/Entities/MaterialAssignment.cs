using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

/// <summary>
/// Represents assignment of materials from warehouse to specific team or employee
/// </summary>
public class MaterialAssignment : BaseAuditableEntity
{
    [Required]
    public int MaterialRequestId { get; set; }

    [Required]
    public int WarehouseItemId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    public int? AssignedToTeamId { get; set; }

    public int? AssignedToEmployeeId { get; set; }

    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Confirms that team/employee received the materials
    /// </summary>
    public bool ReceivedConfirmed { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public int? ReceivedByUserId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(MaterialRequestId))]
    public MaterialRequest MaterialRequest { get; set; } = null!;

    [ForeignKey(nameof(WarehouseItemId))]
    public WarehouseItem WarehouseItem { get; set; } = null!;

    [ForeignKey(nameof(AssignedToTeamId))]
    public Team? AssignedToTeam { get; set; }

    [ForeignKey(nameof(AssignedToEmployeeId))]
    public User? AssignedToEmployee { get; set; }

    [ForeignKey(nameof(ReceivedByUserId))]
    public User? ReceivedByUser { get; set; }
}
