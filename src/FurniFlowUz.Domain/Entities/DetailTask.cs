using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

/// <summary>
/// Represents a detailed task assigned to an employee for a specific furniture detail/part
/// </summary>
public class DetailTask : BaseAuditableEntity
{
    [Required]
    public int CategoryAssignmentId { get; set; }

    public int? DetailId { get; set; }

    public int? AssignedEmployeeId { get; set; }

    /// <summary>
    /// Sequence number for task ordering (1, 2, 3, ...)
    /// </summary>
    [Required]
    public int Sequence { get; set; }

    /// <summary>
    /// ID of the task that must be completed before this one can start
    /// </summary>
    public int? DependsOnTaskId { get; set; }

    [Required]
    [MaxLength(500)]
    public string TaskDescription { get; set; } = string.Empty;

    [Required]
    public DetailTaskStatus Status { get; set; } = DetailTaskStatus.Pending;

    public TimeSpan? EstimatedDuration { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CategoryAssignmentId))]
    public CategoryAssignment CategoryAssignment { get; set; } = null!;

    [ForeignKey(nameof(DetailId))]
    public Detail? Detail { get; set; }

    [ForeignKey(nameof(AssignedEmployeeId))]
    public User? AssignedEmployee { get; set; }

    [ForeignKey(nameof(DependsOnTaskId))]
    public DetailTask? DependsOnTask { get; set; }

    public TaskPerformance? Performance { get; set; }
}
