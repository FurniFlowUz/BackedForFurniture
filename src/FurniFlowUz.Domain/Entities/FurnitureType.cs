using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class FurnitureType : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int OrderId { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProgressPercentage { get; set; } = 0;

    public int? TechnicalSpecificationId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(TechnicalSpecificationId))]
    public TechnicalSpecification? TechnicalSpecification { get; set; }

    public ICollection<Detail> Details { get; set; } = new List<Detail>();
    public ICollection<Drawing> Drawings { get; set; } = new List<Drawing>();
    public ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();
}
