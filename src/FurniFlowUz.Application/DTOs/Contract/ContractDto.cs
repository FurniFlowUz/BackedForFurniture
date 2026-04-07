using FurniFlowUz.Application.DTOs.Auth;
using FurniFlowUz.Application.DTOs.Category;
using FurniFlowUz.Application.DTOs.Customer;

namespace FurniFlowUz.Application.DTOs.Contract;

/// <summary>
/// Full contract details DTO
/// </summary>
public class ContractDto
{
    /// <summary>
    /// Unique contract identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Contract number (system-generated)
    /// </summary>
    public string ContractNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer associated with this contract
    /// </summary>
    public CustomerDto Customer { get; set; } = null!;

    /// <summary>
    /// Customer ID for JSON serialization
    /// </summary>
    public int CustomerId => Customer?.Id ?? 0;

    /// <summary>
    /// Customer name for JSON serialization
    /// </summary>
    public string CustomerName => Customer?.FullName ?? string.Empty;

    /// <summary>
    /// Category identifiers for this contract
    /// </summary>
    public List<int> CategoryIds { get; set; } = new List<int>();

    /// <summary>
    /// DEPRECATED: Use CategoryIds instead. Returns first category ID for backward compatibility.
    /// </summary>
    public int? CategoryId => CategoryIds.Any() ? CategoryIds.First() : null;

    /// <summary>
    /// DEPRECATED: Category name for backward compatibility (not populated)
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Total contract amount
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Advance payment amount
    /// </summary>
    public decimal AdvancePaymentAmount { get; set; }

    /// <summary>
    /// DEPRECATED: Advance payment percentage for backward compatibility. Calculated from amounts.
    /// </summary>
    public decimal? AdvancePaymentPercentage => TotalAmount > 0 ? (AdvancePaymentAmount / TotalAmount) * 100 : null;

    /// <summary>
    /// Deadline date for production completion
    /// </summary>
    public DateTime DeadlineDate { get; set; }

    /// <summary>
    /// DEPRECATED: Description field for backward compatibility. Maps to AdditionalNotes.
    /// </summary>
    public string? Description => AdditionalNotes;

    /// <summary>
    /// DEPRECATED: Notes field for backward compatibility. Maps to AdditionalNotes.
    /// </summary>
    public string? Notes => AdditionalNotes;

    /// <summary>
    /// DEPRECATED: Terms field for backward compatibility. Combines DeliveryTerms and PenaltyTerms.
    /// </summary>
    public string? Terms => string.IsNullOrEmpty(DeliveryTerms) && string.IsNullOrEmpty(PenaltyTerms)
        ? null
        : $"{DeliveryTerms}\n{PenaltyTerms}".Trim();

    /// <summary>
    /// Remaining amount to be paid
    /// </summary>
    public decimal RemainingAmount { get; set; }

    /// <summary>
    /// Contract signed date
    /// </summary>
    public DateTime? SignedDate { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Contract status (New, Active, Completed, Cancelled)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Delivery terms and conditions
    /// </summary>
    public string? DeliveryTerms { get; set; }

    /// <summary>
    /// Penalty terms and conditions
    /// </summary>
    public string? PenaltyTerms { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? AdditionalNotes { get; set; }

    /// <summary>
    /// Requires approval flag
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// User who created the contract
    /// </summary>
    public UserDto? CreatedBy { get; set; }

    /// <summary>
    /// Seller/Salesperson ID - maps to CreatedBy for backward compatibility
    /// </summary>
    public int SellerId => CreatedBy?.Id ?? 0;

    /// <summary>
    /// Seller/Salesperson name - maps to CreatedBy for backward compatibility
    /// </summary>
    public string SellerName => CreatedBy?.FullName ?? string.Empty;

    /// <summary>
    /// Date and time when contract was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when contract was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
