using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class ProductionStage : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ProductionStageType StageType { get; set; }

    public int SequenceOrder { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? EstimatedDurationHours { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();
}
