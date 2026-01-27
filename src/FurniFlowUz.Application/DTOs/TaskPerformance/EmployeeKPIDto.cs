namespace FurniFlowUz.Application.DTOs.TaskPerformance;

/// <summary>
/// Employee KPI metrics DTO
/// </summary>
public class EmployeeKPIDto
{
    /// <summary>
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Employee full name
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Position name
    /// </summary>
    public string PositionName { get; set; } = string.Empty;

    /// <summary>
    /// Department name
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Total tasks completed
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
    /// Average efficiency percentage
    /// </summary>
    public decimal AverageEfficiency { get; set; }

    /// <summary>
    /// Average quality score (1-10)
    /// </summary>
    public decimal AverageQualityScore { get; set; }

    /// <summary>
    /// On-time completion rate percentage
    /// </summary>
    public decimal OnTimeCompletionRate { get; set; }

    /// <summary>
    /// Percentage of tasks requiring rework
    /// </summary>
    public decimal ReworkRate { get; set; }

    /// <summary>
    /// Current active tasks count
    /// </summary>
    public int ActiveTasksCount { get; set; }

    /// <summary>
    /// Tasks in queue (pending/ready)
    /// </summary>
    public int PendingTasksCount { get; set; }

    /// <summary>
    /// Period start date for metrics
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Period end date for metrics
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}
