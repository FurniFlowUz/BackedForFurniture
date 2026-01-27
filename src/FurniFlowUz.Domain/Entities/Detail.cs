using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class Detail : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Width { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Height { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Thickness { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public int FurnitureTypeId { get; set; }

    [MaxLength(200)]
    public string? Material { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(FurnitureTypeId))]
    public FurnitureType FurnitureType { get; set; } = null!;
}
