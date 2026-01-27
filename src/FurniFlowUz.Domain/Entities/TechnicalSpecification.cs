using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class TechnicalSpecification : BaseAuditableEntity
{
    [MaxLength(2000)]
    public string? Notes { get; set; }

    public bool IsLocked { get; set; } = false;

    [Required]
    public int FurnitureTypeId { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(FurnitureTypeId))]
    public FurnitureType FurnitureType { get; set; } = null!;
}
