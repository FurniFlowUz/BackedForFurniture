using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

/// <summary>
/// Template for furniture types that Production Manager creates
/// Constructor can select these templates when creating furniture types for orders
/// </summary>
public class FurnitureTypeTemplate : BaseAuditableEntity
{
    /// <summary>
    /// Template name (e.g., "2 eshikli shkaf", "3 eshikli shkaf")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category this template belongs to
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Description of this furniture type template
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Default material suggestion for this template
    /// </summary>
    [MaxLength(200)]
    public string? DefaultMaterial { get; set; }

    /// <summary>
    /// Default notes or specifications
    /// </summary>
    [MaxLength(2000)]
    public string? DefaultNotes { get; set; }

    /// <summary>
    /// Whether this template is active and can be used
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Display order for sorting templates
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Furniture types created from this template
    /// </summary>
    public ICollection<FurnitureType> FurnitureTypes { get; set; } = new List<FurnitureType>();
}
