namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// Order statistics DTO
/// </summary>
public class OrderStatsDto
{
    /// <summary>
    /// Total number of orders
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Number of orders with New or Assigned status
    /// </summary>
    public int Created { get; set; }

    /// <summary>
    /// Number of orders in progress (SpecificationsReady, InProduction, QualityCheck)
    /// </summary>
    public int InProgress { get; set; }

    /// <summary>
    /// Number of completed orders (Completed, Delivered)
    /// </summary>
    public int Completed { get; set; }
}
