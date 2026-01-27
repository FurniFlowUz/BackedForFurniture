namespace FurniFlowUz.Application.DTOs.Seller;

/// <summary>
/// Contract statistics for seller dashboard
/// </summary>
public class ContractStatsDto
{
    /// <summary>
    /// Number of active contracts
    /// </summary>
    public int ActiveContracts { get; set; }

    /// <summary>
    /// Number of pending orders awaiting fulfillment
    /// </summary>
    public int PendingOrders { get; set; }

    /// <summary>
    /// Number of completed orders
    /// </summary>
    public int CompletedOrders { get; set; }

    /// <summary>
    /// Total revenue from all contracts
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Revenue change percentage from previous period
    /// </summary>
    public decimal RevenueChangePercentage { get; set; }

    /// <summary>
    /// Active contracts change percentage from previous period
    /// </summary>
    public decimal ActiveContractsChangePercentage { get; set; }

    /// <summary>
    /// Pending orders change percentage from previous period
    /// </summary>
    public decimal PendingOrdersChangePercentage { get; set; }
}
