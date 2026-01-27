using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    [Required]
    public NotificationType Type { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public int? UserId { get; set; }

    public UserRole? Role { get; set; }

    public bool IsRead { get; set; } = false;

    [MaxLength(100)]
    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
