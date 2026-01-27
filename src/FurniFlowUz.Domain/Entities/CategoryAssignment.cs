using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

/// <summary>
/// Represents assignment of a furniture category to a team leader for production
/// </summary>
public class CategoryAssignment : BaseAuditableEntity
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int FurnitureTypeId { get; set; }

    [Required]
    public int TeamLeaderId { get; set; }

    [Required]
    public int TeamId { get; set; }

    [Required]
    public CategoryAssignmentStatus Status { get; set; } = CategoryAssignmentStatus.Assigned;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(FurnitureTypeId))]
    public FurnitureType FurnitureType { get; set; } = null!;

    [ForeignKey(nameof(TeamLeaderId))]
    public User TeamLeader { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    public Team Team { get; set; } = null!;

    public ICollection<DetailTask> DetailTasks { get; set; } = new List<DetailTask>();
}
