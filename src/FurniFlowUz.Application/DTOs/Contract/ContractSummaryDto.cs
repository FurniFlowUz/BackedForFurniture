namespace FurniFlowUz.Application.DTOs.Contract;

/// <summary>
/// Brief contract information for list views
/// </summary>
public class ContractSummaryDto
{
    /// <summary>
    /// Unique contract identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Contract number
    /// </summary>
    public string ContractNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Seller (creator) full name
    /// </summary>
    public string? SellerName { get; set; }

    /// <summary>
    /// Category names (display-ready)
    /// </summary>
    public List<string> CategoryNames { get; set; } = new List<string>();

    /// <summary>
    /// Category identifiers for this contract
    /// </summary>
    public List<int> CategoryIds { get; set; } = new List<int>();

    /// <summary>
    /// DEPRECATED: Use CategoryIds instead. Returns first category ID for backward compatibility.
    /// </summary>
    public int? CategoryId => CategoryIds.Any() ? CategoryIds.First() : null;

    /// <summary>
    /// Total contract amount
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// DEPRECATED: Advance payment percentage for backward compatibility. Calculated from amounts.
    /// </summary>
    public decimal? AdvancePaymentPercentage => TotalAmount > 0 ? (AdvancePaymentAmount / TotalAmount) * 100 : null;

    /// <summary>
    /// Advance payment amount
    /// </summary>
    public decimal AdvancePaymentAmount { get; set; }

    /// <summary>
    /// Remaining amount to be paid
    /// </summary>
    public decimal RemainingAmount { get; set; }

    /// <summary>
    /// Production duration in days
    /// </summary>
    public int ProductionDurationDays { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Contract status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Requires approval flag
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Date when contract was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
