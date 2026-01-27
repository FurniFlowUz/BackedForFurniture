using FurniFlowUz.Application.DTOs.Auth;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// Warehouse transaction details DTO
/// </summary>
public class WarehouseTransactionDto
{
    /// <summary>
    /// Unique transaction identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Warehouse item ID
    /// </summary>
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Warehouse item name
    /// </summary>
    public string WarehouseItemName { get; set; } = string.Empty;

    /// <summary>
    /// Warehouse item SKU
    /// </summary>
    public string WarehouseItemSKU { get; set; } = string.Empty;

    /// <summary>
    /// Transaction type (Income or Outcome)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Quantity involved in transaction
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Team ID (for outcome transactions)
    /// </summary>
    public int? TeamId { get; set; }

    /// <summary>
    /// Team name (for outcome transactions)
    /// </summary>
    public string? TeamName { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User ID who created the transaction
    /// </summary>
    public int CreatedByUserId { get; set; }

    /// <summary>
    /// User name who created the transaction
    /// </summary>
    public string CreatedByUserName { get; set; } = string.Empty;

    /// <summary>
    /// Transaction date
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Date and time when transaction was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
