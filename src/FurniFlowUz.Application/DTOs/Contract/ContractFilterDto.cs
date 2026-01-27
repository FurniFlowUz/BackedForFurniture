using FurniFlowUz.Application.DTOs.Common;

namespace FurniFlowUz.Application.DTOs.Contract;

/// <summary>
/// Filter DTO for contract queries
/// </summary>
public class ContractFilterDto : BaseFilter
{
    /// <summary>
    /// Filter by contract status
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
    /// Filter contracts created from this date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter contracts created up to this date
    /// </summary>
    public DateTime? ToDate { get; set; }
}
