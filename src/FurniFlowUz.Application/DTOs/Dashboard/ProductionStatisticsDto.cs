namespace FurniFlowUz.Application.DTOs.Dashboard;

/// <summary>
/// Production metrics and statistics DTO
/// </summary>
public class ProductionStatisticsDto
{
    /// <summary>
    /// Total number of tasks
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of pending tasks
    /// </summary>
    public int PendingTasks { get; set; }

    /// <summary>
    /// Number of tasks in progress
    /// </summary>
    public int InProgressTasks { get; set; }

    /// <summary>
    /// Number of completed tasks
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Number of delayed tasks
    /// </summary>
    public int DelayedTasks { get; set; }

    /// <summary>
    /// Average task completion time in hours
    /// </summary>
    public decimal AverageCompletionTime { get; set; }

    /// <summary>
    /// Production efficiency percentage (actual vs estimated hours)
    /// </summary>
    public decimal EfficiencyPercentage { get; set; }

    /// <summary>
    /// Number of active teams
    /// </summary>
    public int ActiveTeams { get; set; }

    /// <summary>
    /// Number of active workers
    /// </summary>
    public int ActiveWorkers { get; set; }

    /// <summary>
    /// Task completion rate per day
    /// </summary>
    public Dictionary<string, int> DailyCompletionRate { get; set; } = new();
}
