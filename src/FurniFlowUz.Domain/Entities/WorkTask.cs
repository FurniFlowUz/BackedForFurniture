using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class WorkTask : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public int OrderId { get; set; }

    public int? FurnitureTypeId { get; set; }

    [Required]
    public int ProductionStageId { get; set; }

    [Required]
    public int TeamId { get; set; }

    public int? AssignedWorkerId { get; set; }

    public int SequenceOrder { get; set; }

    [Required]
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;

    [Column(TypeName = "decimal(10,2)")]
    public decimal? EstimatedHours { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? ActualHours { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(FurnitureTypeId))]
    public FurnitureType? FurnitureType { get; set; }

    [ForeignKey(nameof(ProductionStageId))]
    public ProductionStage ProductionStage { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    public Team Team { get; set; } = null!;

    [ForeignKey(nameof(AssignedWorkerId))]
    public User? AssignedWorker { get; set; }
}
