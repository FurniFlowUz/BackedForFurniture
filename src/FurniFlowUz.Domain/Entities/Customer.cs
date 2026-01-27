using System.ComponentModel.DataAnnotations;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
