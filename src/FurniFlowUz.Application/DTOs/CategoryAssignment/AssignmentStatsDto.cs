namespace FurniFlowUz.Application.DTOs.CategoryAssignment;

/// <summary>
/// DTO for category assignment statistics (Production Manager view)
/// </summary>
public class AssignmentStatsDto
{
    /// <summary>
    /// Total number of category assignments
    /// </summary>
    public int TotalAssignments { get; set; }

    /// <summary>
    /// Number of pending assignments (not started)
    /// </summary>
    public int PendingAssignments { get; set; }

    /// <summary>
    /// Number of in-progress assignments
    /// </summary>
    public int InProgressAssignments { get; set; }

    /// <summary>
    /// Number of completed assignments
    /// </summary>
    public int CompletedAssignments { get; set; }

    /// <summary>
    /// Number of overdue assignments (past deadline)
    /// </summary>
    public int OverdueAssignments { get; set; }

    /// <summary>
    /// Average completion percentage across all assignments
    /// </summary>
    public decimal AverageCompletionPercentage { get; set; }

    /// <summary>
    /// Number of active team leaders with assignments
    /// </summary>
    public int ActiveTeamLeaders { get; set; }

    /// <summary>
    /// List of recent assignments (last 10)
    /// </summary>
    public List<CategoryAssignmentSummaryDto> RecentAssignments { get; set; } = new();
}
