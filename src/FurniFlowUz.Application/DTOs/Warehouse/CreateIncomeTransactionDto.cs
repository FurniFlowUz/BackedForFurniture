using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// DTO for creating an income (stock receiving) transaction
/// </summary>
public class CreateIncomeTransactionDto
{
    /// <summary>
    /// Warehouse item identifier
    /// </summary>
    [Required(ErrorMessage = "Warehouse item ID is required")]
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Quantity received
    /// </summary>
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    /// <summary>
    /// User ID who created the transaction
    /// </summary>
    [Required(ErrorMessage = "Created by user ID is required")]
    public int CreatedByUserId { get; set; }

    /// <summary>
    /// Supplier name
    /// </summary>
    [MaxLength(200)]
    public string? Supplier { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}
