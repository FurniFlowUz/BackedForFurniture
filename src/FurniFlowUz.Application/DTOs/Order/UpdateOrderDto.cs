using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// DTO for updating an existing order
/// </summary>
public class UpdateOrderDto
{
    /// <summary>
    /// Contract ID (cannot be changed for existing orders, kept for reference)
    /// </summary>
    public int? ContractId { get; set; }

    /// <summary>
    /// Order description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Category IDs for this order
    /// </summary>
    public List<int>? CategoryIds { get; set; }
}
