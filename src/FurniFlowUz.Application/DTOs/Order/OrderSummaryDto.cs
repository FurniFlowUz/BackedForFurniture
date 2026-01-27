namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// Brief order information for list views
/// </summary>
public class OrderSummaryDto
{
    /// <summary>
    /// Unique order identifier
    /// </summary>
    public int? Id { get; set; }

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
    /// Number of furniture types in this order
    /// </summary>
    public int FurnitureTypesCount { get; set; }

    /// <summary>
    /// Expected delivery date
    /// </summary>
    public DateTime ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// Order status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Assigned constructor name
    /// </summary>
    public string? AssignedConstructorName { get; set; }

    /// <summary>
    /// Assigned production manager name
    /// </summary>
    public string? AssignedProductionManagerName { get; set; }

    /// <summary>
    /// Date when order was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
