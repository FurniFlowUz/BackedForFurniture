using FurniFlowUz.Application.DTOs.Dashboard;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for dashboard statistics and analytics
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets comprehensive dashboard data for the Director role
    /// </summary>
    Task<DirectorDashboardDto> GetDirectorDashboardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets production-focused dashboard data for the ProductionManager role
    /// Shows ONLY production-related data (NO sales revenue, NO global stats)
    /// </summary>
    Task<ProductionManagerDashboardDto> GetProductionManagerDashboardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets general statistics (total orders, active contracts, etc.)
    /// </summary>
    Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets revenue statistics (daily, monthly, total)
    /// </summary>
    Task<RevenueStatisticsDto> GetRevenueStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets production statistics (completed tasks, active tasks, etc.)
    /// </summary>
    Task<ProductionStatisticsDto> GetProductionStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets delayed tasks and orders (past deadline and not completed)
    /// </summary>
    Task<IEnumerable<DelayedTaskDto>> GetDelayedTasksAsync(CancellationToken cancellationToken = default);
}
