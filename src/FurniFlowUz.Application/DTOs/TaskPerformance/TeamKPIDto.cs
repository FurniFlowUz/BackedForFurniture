namespace FurniFlowUz.Application.DTOs.TaskPerformance;

/// <summary>
/// Team KPI metrics DTO
/// </summary>
public class TeamKPIDto
{
    /// <summary>
    /// Team ID
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// Team leader name
    /// </summary>
    public string TeamLeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Total team members
    /// </summary>
    public int TotalMembers { get; set; }

    /// <summary>
    /// Active team members
    /// </summary>
    public int ActiveMembers { get; set; }

    /// <summary>
    /// Total tasks completed by team
    /// </summary>
    public int TotalTasksCompleted { get; set; }

    /// <summary>
    /// Tasks completed today
    /// </summary>
    public int TasksCompletedToday { get; set; }

    /// <summary>
    /// Tasks completed this week
    /// </summary>
    public int TasksCompletedThisWeek { get; set; }

    /// <summary>
    /// Tasks completed this month
    /// </summary>
    public int TasksCompletedThisMonth { get; set; }

    /// <summary>
    /// Average team efficiency percentage
    /// </summary>
    public decimal AverageEfficiency { get; set; }

    /// <summary>
    /// Average team quality score (1-10)
    /// </summary>
    public decimal AverageQualityScore { get; set; }

    /// <summary>
    /// Team on-time completion rate
    /// </summary>
    public decimal OnTimeCompletionRate { get; set; }

    /// <summary>
    /// Team rework rate
    /// </summary>
    public decimal ReworkRate { get; set; }

    /// <summary>
    /// Current active assignments
    /// </summary>
    public int ActiveAssignmentsCount { get; set; }

    /// <summary>
    /// Completed assignments count
    /// </summary>
    public int CompletedAssignmentsCount { get; set; }

    /// <summary>
    /// Top performing employees
    /// </summary>
    public List<EmployeePerformanceSummary> TopPerformers { get; set; } = new();

    /// <summary>
    /// Period start date for metrics
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Period end date for metrics
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// Summary of employee performance for team rankings
/// </summary>
public class EmployeePerformanceSummary
{
    /// <summary>
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Employee name
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Tasks completed
    /// </summary>
    public int TasksCompleted { get; set; }

    /// <summary>
    /// Average efficiency
    /// </summary>
    public decimal AverageEfficiency { get; set; }

    /// <summary>
    /// Average quality score
    /// </summary>
    public decimal AverageQualityScore { get; set; }
}
