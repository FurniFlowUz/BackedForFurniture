using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class AuditLog : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty;

    [Required]
    public int EntityId { get; set; }

    [MaxLength(4000)]
    public string? OldValues { get; set; }

    [MaxLength(4000)]
    public string? NewValues { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
