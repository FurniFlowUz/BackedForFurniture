namespace FurniFlowUz.Application.DTOs.Dashboard;

/// <summary>
/// Production Manager Dashboard DTO - Shows ONLY production-related data
/// NO sales revenue, NO global company stats, NO other managers' data
/// </summary>
public class ProductionManagerDashboardDto
{
    /// <summary>
    /// Total orders assigned to this ProductionManager
    /// </summary>
    public int TotalAssignedOrders { get; set; }

    /// <summary>
    /// Orders in SpecificationsReady status (ready to start production)
    /// </summary>
    public int OrdersReadyForProduction { get; set; }

    /// <summary>
    /// Orders currently in production
    /// </summary>
    public int OrdersInProduction { get; set; }

    /// <summary>
    /// Orders in quality check phase
    /// </summary>
    public int OrdersInQualityCheck { get; set; }

    /// <summary>
    /// Orders completed
    /// </summary>
    public int OrdersCompleted { get; set; }

    /// <summary>
    /// Orders past deadline (delayed)
    /// </summary>
    public int DelayedOrders { get; set; }

    /// <summary>
    /// Total tasks assigned by/to this ProductionManager
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Tasks in progress
    /// </summary>
    public int TasksInProgress { get; set; }

    /// <summary>
    /// Tasks completed
    /// </summary>
    public int TasksCompleted { get; set; }

    /// <summary>
    /// Tasks pending assignment
    /// </summary>
    public int TasksPending { get; set; }

    /// <summary>
    /// Average production progress across all assigned orders
    /// </summary>
    public decimal AverageProgress { get; set; }

    /// <summary>
    /// Production efficiency percentage (estimated vs actual hours)
    /// </summary>
    public decimal EfficiencyPercentage { get; set; }

    /// <summary>
    /// Number of active workers under this ProductionManager
    /// </summary>
    public int ActiveWorkers { get; set; }

    /// <summary>
    /// Number of active teams under this ProductionManager
    /// </summary>
    public int ActiveTeams { get; set; }

    /// <summary>
    /// Delayed tasks that need immediate attention
    /// </summary>
    public List<DelayedTaskDto> DelayedTasks { get; set; } = new();

    /// <summary>
    /// Constructors working under this ProductionManager
    /// </summary>
    public List<ConstructorSummaryDto> Constructors { get; set; } = new();
}

/// <summary>
/// Summary information for a Constructor
/// </summary>
public class ConstructorSummaryDto
{
    /// <summary>
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Number of orders assigned
    /// </summary>
    public int AssignedOrdersCount { get; set; }

    /// <summary>
    /// Number of completed orders
    /// </summary>
    public int CompletedOrdersCount { get; set; }
}
