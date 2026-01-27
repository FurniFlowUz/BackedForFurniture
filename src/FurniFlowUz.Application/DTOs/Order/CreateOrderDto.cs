using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// DTO for creating a new order from a contract
/// Customer and Categories are automatically derived from the Contract
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// Contract identifier (REQUIRED - Order must be created from a Contract)
    /// Customer and Categories will be derived from this Contract
    /// </summary>
    [Required(ErrorMessage = "Contract is required")]
    public int ContractId { get; set; }

    /// <summary>
    /// Order description (optional - can describe specific items or customizations)
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }
}
