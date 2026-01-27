namespace FurniFlowUz.Application.DTOs.Dashboard;

/// <summary>
/// Delayed task information with delay details
/// </summary>
public class DelayedTaskDto
{
    /// <summary>
    /// Task identifier
    /// </summary>
    public int? TaskId { get; set; }

    /// <summary>
    /// Task title
    /// </summary>
    public string TaskTitle { get; set; } = string.Empty;

    /// <summary>
    /// Order number
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Team name
    /// </summary>
    public string? TeamName { get; set; }

    /// <summary>
    /// Assigned worker name
    /// </summary>
    public string? AssignedWorkerName { get; set; }

    /// <summary>
    /// Expected completion date
    /// </summary>
    public DateTime ExpectedCompletionDate { get; set; }

    /// <summary>
    /// Number of days delayed
    /// </summary>
    public int DaysDelayed { get; set; }

    /// <summary>
    /// Current task status
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
