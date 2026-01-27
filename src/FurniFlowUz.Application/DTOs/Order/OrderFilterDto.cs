using FurniFlowUz.Application.DTOs.Common;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// Filter DTO for order queries
/// </summary>
public class OrderFilterDto : BaseFilter
{
    /// <summary>
    /// Filter by order status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by customer identifier
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Filter by category identifier
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filter orders created from this date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter orders created up to this date
    /// </summary>
    public DateTime? ToDate { get; set; }
}
