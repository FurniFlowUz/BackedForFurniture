using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

/// <summary>
/// Tracks performance metrics for completed tasks to calculate employee KPIs
/// </summary>
public class TaskPerformance : BaseAuditableEntity
{
    [Required]
    public int DetailTaskId { get; set; }

    [Required]
    public TimeSpan ActualDuration { get; set; }

    /// <summary>
    /// Efficiency percentage: (Estimated / Actual) * 100
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal EfficiencyPercent { get; set; }

    /// <summary>
    /// Quality score from 1-10
    /// </summary>
    [Range(1, 10)]
    public int QualityScore { get; set; } = 10;

    /// <summary>
    /// Whether task required rework due to quality issues
    /// </summary>
    public bool RequiredRework { get; set; }

    [MaxLength(500)]
    public string? ReworkReason { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(DetailTaskId))]
    public DetailTask DetailTask { get; set; } = null!;
}
