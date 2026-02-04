using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Contract;

/// <summary>
/// DTO for updating an existing contract
/// </summary>
public class UpdateContractDto
{
    /// <summary>
    /// Category identifiers (comma-separated)
    /// </summary>
    [Required(ErrorMessage = "At least one category is required")]
    public List<int> CategoryIds { get; set; } = new List<int>();

    /// <summary>
    /// Total contract amount
    /// </summary>
    [Required(ErrorMessage = "Total amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Advance payment amount (exact number)
    /// </summary>
    [Required(ErrorMessage = "Advance payment amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Advance payment amount must be 0 or greater")]
    public decimal AdvancePaymentAmount { get; set; }

    /// <summary>
    /// Deadline date for production completion
    /// </summary>
    [Required(ErrorMessage = "Deadline date is required")]
    public DateTime DeadlineDate { get; set; }

    /// <summary>
    /// Contract signed date
    /// </summary>
    public DateTime? SignedDate { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [MaxLength(50)]
    public string? PaymentStatus { get; set; }

    /// <summary>
    /// Contract status (New, Active, Completed, Cancelled)
    /// </summary>
    [Required(ErrorMessage = "Status is required")]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

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
