using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.DTOs.Warehouse;

namespace FurniFlowUz.Application.DTOs.Dashboard;

/// <summary>
/// Complete dashboard DTO with all statistics for Director role
/// </summary>
public class DirectorDashboardDto
{
    /// <summary>
    /// Total number of orders
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Number of orders currently in progress
    /// </summary>
    public int OrdersInProgress { get; set; }

    /// <summary>
    /// Number of delayed orders (past expected delivery date)
    /// </summary>
    public int DelayedOrders { get; set; }

    /// <summary>
    /// Number of completed orders
    /// </summary>
    public int CompletedOrders { get; set; }

    /// <summary>
    /// Total revenue from all contracts
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Revenue generated today
    /// </summary>
    public decimal DailyRevenue { get; set; }

    /// <summary>
    /// Revenue generated this month
    /// </summary>
    public decimal MonthlyRevenue { get; set; }

    /// <summary>
    /// Number of active workers
    /// </summary>
    public int ActiveWorkers { get; set; }

    /// <summary>
    /// Warehouse alerts for low stock items
    /// </summary>
    public List<WarehouseAlertDto> WarehouseAlerts { get; set; } = new();

    /// <summary>
    /// Recent orders (last 10)
    /// </summary>
    public List<OrderSummaryDto> RecentOrders { get; set; } = new();

    /// <summary>
    /// System alerts and notifications
    /// </summary>
    public List<string> Alerts { get; set; } = new();

    /// <summary>
    /// Delayed tasks that need attention
    /// </summary>
    public List<DelayedTaskDto> DelayedTasks { get; set; } = new();
}
