using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

/// <summary>
/// Many-to-Many relationship between Order and Category
/// </summary>
public class OrderCategory : BaseEntity
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Deadline date for this order-category
    /// </summary>
    public DateTime? DeadlineDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Furniture types belonging to this order-category
    /// </summary>
    public ICollection<FurnitureType> FurnitureTypes { get; set; } = new List<FurnitureType>();
}
