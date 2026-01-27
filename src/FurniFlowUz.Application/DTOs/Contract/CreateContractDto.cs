using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Contract;

/// <summary>
/// DTO for creating a new contract
/// </summary>
public class CreateContractDto
{
    /// <summary>
    /// Existing customer identifier (use this for existing customers)
    /// Either CustomerId OR NewCustomer must be provided, not both
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// New customer data (use this to create a customer inline)
    /// Either CustomerId OR NewCustomer must be provided, not both
    /// </summary>
    public NewCustomerDto? NewCustomer { get; set; }

    /// <summary>
    /// Category identifiers (comma-separated)
    /// </summary>
    public List<int> CategoryIds { get; set; } = new List<int>();

    /// <summary>
    /// DEPRECATED: Single category ID for backward compatibility.
    /// If provided and CategoryIds is empty, it will be converted to CategoryIds.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Total contract amount
    /// </summary>
    [Required(ErrorMessage = "Total amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Advance payment amount (exact number)
    /// </summary>
    public decimal AdvancePaymentAmount { get; set; }

    /// <summary>
    /// DEPRECATED: Advance payment percentage for backward compatibility.
    /// If provided and AdvancePaymentAmount is 0, it will be calculated.
    /// </summary>
    public decimal? AdvancePaymentPercentage { get; set; }

    /// <summary>
    /// Production duration in days (optional, default 30)
    /// </summary>
    public int ProductionDurationDays { get; set; } = 30;

    /// <summary>
    /// DEPRECATED: Deadline for backward compatibility.
    /// If provided, ProductionDurationDays will be calculated from SignedDate.
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// DEPRECATED: Description for backward compatibility. Maps to AdditionalNotes.
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// DEPRECATED: Notes for backward compatibility. Maps to AdditionalNotes.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// DEPRECATED: Terms for backward compatibility. Maps to DeliveryTerms and PenaltyTerms.
    /// </summary>
    [MaxLength(5000)]
    public string? Terms { get; set; }

    /// <summary>
    /// Contract signed date
    /// </summary>
    public DateTime? SignedDate { get; set; }

    /// <summary>
    /// Delivery terms and conditions
    /// </summary>
    [MaxLength(2000)]
    public string? DeliveryTerms { get; set; }

    /// <summary>
    /// Penalty terms and conditions
    /// </summary>
    [MaxLength(2000)]
    public string? PenaltyTerms { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(2000)]
    public string? AdditionalNotes { get; set; }
}
