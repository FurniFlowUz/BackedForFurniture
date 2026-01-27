using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class Order : BaseAuditableEntity
{
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public int? ContractId { get; set; }

    [Required]
    public DateTime DeadlineDate { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.New;

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProgressPercentage { get; set; } = 0;

    public int? AssignedConstructorId { get; set; }

    public int? AssignedProductionManagerId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    [ForeignKey(nameof(ContractId))]
    public Contract? Contract { get; set; }

    [ForeignKey(nameof(AssignedConstructorId))]
    public User? AssignedConstructor { get; set; }

    [ForeignKey(nameof(AssignedProductionManagerId))]
    public User? AssignedProductionManager { get; set; }

    public ICollection<FurnitureType> FurnitureTypes { get; set; } = new List<FurnitureType>();
    public ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();
}
