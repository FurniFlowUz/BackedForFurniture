using FurniFlowUz.Application.DTOs.Seller;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for seller/salesperson-specific operations
/// PERFORMANCE OPTIMIZED - No CancellationToken to avoid TaskCanceledException
/// Uses efficient aggregate queries instead of loading all data
/// </summary>
public interface ISellerService
{
    /// <summary>
    /// Gets dashboard statistics for the seller using EFFICIENT AGGREGATE QUERIES
    /// NO CancellationToken - avoids TaskCanceledException on dashboard loads
    /// </summary>
    /// <returns>Dashboard statistics including active contracts, pending orders, revenue, etc.</returns>
    Task<ContractStatsDto> GetDashboardStatsAsync();

    /// <summary>
    /// Gets recent activities for the seller using EFFICIENT QUERIES
    /// NO CancellationToken - avoids TaskCanceledException
    /// </summary>
    /// <param name="limit">Maximum number of activities to retrieve</param>
    /// <returns>List of recent activities</returns>
    Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(int limit = 10);

    /// <summary>
    /// Gets pending items requiring seller attention using EFFICIENT QUERIES
    /// NO CancellationToken - avoids TaskCanceledException
    /// </summary>
    /// <returns>List of pending items</returns>
    Task<IEnumerable<PendingItemDto>> GetPendingItemsAsync();
}
