using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class Contract : BaseAuditableEntity
{
    [Required]
    [MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;

    [Required]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(500)]
    public string CategoryIds { get; set; } = string.Empty; // Comma-separated category IDs

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AdvancePaymentAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RemainingAmount { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [Required]
    public ContractStatus Status { get; set; } = ContractStatus.New;

    [Required]
    public DateTime DeadlineDate { get; set; }

    public DateTime? SignedDate { get; set; }

    [MaxLength(2000)]
    public string? DeliveryTerms { get; set; }

    [MaxLength(2000)]
    public string? PenaltyTerms { get; set; }

    [MaxLength(2000)]
    public string? AdditionalNotes { get; set; }

    public bool RequiresApproval { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
