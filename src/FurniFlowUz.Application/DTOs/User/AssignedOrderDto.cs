using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.User;

/// <summary>
/// Simple DTO for assigned orders (for constructor/production manager use)
/// </summary>
public class AssignedOrderDto
{
    /// <summary>
    /// Order ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Order number
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Order status
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Deadline date
    /// </summary>
    public DateTime DeadlineDate { get; set; }

    /// <summary>
    /// Number of furniture types in this order
    /// </summary>
    public int FurnitureTypesCount { get; set; }

    /// <summary>
    /// Date when order was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
}
