using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class Employee : BaseAuditableEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    public int PositionId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;

    public int ActiveTasks { get; set; } = 0;

    public int CompletedTasks { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal? OnTimePercent { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(PositionId))]
    public Position Position { get; set; } = null!;

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;
}
