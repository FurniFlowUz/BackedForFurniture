namespace FurniFlowUz.Application.DTOs.Dashboard;

/// <summary>
/// General statistics DTO
/// </summary>
public class StatisticsDto
{
    /// <summary>
    /// Total count
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Count for current period
    /// </summary>
    public int CurrentPeriodCount { get; set; }

    /// <summary>
    /// Count for previous period
    /// </summary>
    public int PreviousPeriodCount { get; set; }

    /// <summary>
    /// Percentage change from previous period
    /// </summary>
    public decimal PercentageChange { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
