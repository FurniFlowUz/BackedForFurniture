using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class Drawing : BaseAuditableEntity
{
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    public int FurnitureTypeId { get; set; }

    [Required]
    public DateTime UploadedAt { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation properties
    [ForeignKey(nameof(FurnitureTypeId))]
    public FurnitureType FurnitureType { get; set; } = null!;
}
