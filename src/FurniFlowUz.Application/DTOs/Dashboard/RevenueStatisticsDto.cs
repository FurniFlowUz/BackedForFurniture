namespace FurniFlowUz.Application.DTOs.Dashboard;

/// <summary>
/// Revenue statistics by period DTO
/// </summary>
public class RevenueStatisticsDto
{
    /// <summary>
    /// Total revenue
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Revenue for today
    /// </summary>
    public decimal DailyRevenue { get; set; }

    /// <summary>
    /// Revenue for current week
    /// </summary>
    public decimal WeeklyRevenue { get; set; }

    /// <summary>
    /// Revenue for current month
    /// </summary>
    public decimal MonthlyRevenue { get; set; }

    /// <summary>
    /// Revenue for current year
    /// </summary>
    public decimal YearlyRevenue { get; set; }

    /// <summary>
    /// Revenue breakdown by month (for charts)
    /// </summary>
    public Dictionary<string, decimal> MonthlyBreakdown { get; set; } = new();

    /// <summary>
    /// Revenue breakdown by category
    /// </summary>
    public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
}
