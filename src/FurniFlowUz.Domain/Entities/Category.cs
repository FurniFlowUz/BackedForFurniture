using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class Category : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RetailPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WholesalePrice { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal MinAdvancePercent { get; set; }

    public int StandardProductionDays { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// Templates for furniture types in this category
    /// </summary>
    public ICollection<FurnitureTypeTemplate> FurnitureTypeTemplates { get; set; } = new List<FurnitureTypeTemplate>();
}
