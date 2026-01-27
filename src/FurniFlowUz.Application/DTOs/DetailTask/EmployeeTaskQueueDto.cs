namespace FurniFlowUz.Application.DTOs.DetailTask;

/// <summary>
/// DTO for employee's task queue view
/// </summary>
public class EmployeeTaskQueueDto
{
    /// <summary>
    /// Current task the employee should work on (if any)
    /// </summary>
    public DetailTaskDto? CurrentTask { get; set; }

    /// <summary>
    /// Upcoming tasks that are ready to start
    /// </summary>
    public List<DetailTaskListDto> UpcomingTasks { get; set; } = new();

    /// <summary>
    /// Tasks locked waiting for dependencies
    /// </summary>
    public List<DetailTaskListDto> LockedTasks { get; set; } = new();

    /// <summary>
    /// Total tasks assigned to employee
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Completed tasks count
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Tasks in progress count
    /// </summary>
    public int InProgressTasks { get; set; }
}
