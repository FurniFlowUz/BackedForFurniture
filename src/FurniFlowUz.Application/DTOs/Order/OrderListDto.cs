namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// DTO for orders list view with complete display-ready data
/// </summary>
public class OrderListDto
{
    /// <summary>
    /// Unique order identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Order number
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Associated contract ID
    /// </summary>
    public int? ContractId { get; set; }

    /// <summary>
    /// Contract number (display-ready)
    /// </summary>
    public string? ContractNumber { get; set; }

    /// <summary>
    /// Customer name (display-ready)
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Category names (display-ready, comma-separated from contract)
    /// </summary>
    public List<string> CategoryNames { get; set; } = new List<string>();

    /// <summary>
    /// Total amount (derived from contract or calculated)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Order status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Assigned constructor name
    /// </summary>
    public string? AssignedConstructorName { get; set; }

    /// <summary>
    /// Assigned production manager name
    /// </summary>
    public string? AssignedProductionManagerName { get; set; }

    /// <summary>
    /// Deadline date
    /// </summary>
    public DateTime DeadlineDate { get; set; }

    /// <summary>
    /// Date when order was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
