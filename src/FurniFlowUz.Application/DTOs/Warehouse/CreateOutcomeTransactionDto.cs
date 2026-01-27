using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// DTO for creating an outcome (stock withdrawal) transaction
/// </summary>
public class CreateOutcomeTransactionDto
{
    /// <summary>
    /// Warehouse item identifier
    /// </summary>
    [Required(ErrorMessage = "Warehouse item ID is required")]
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Quantity withdrawn
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
    /// Team receiving the materials
    /// </summary>
    [Required(ErrorMessage = "Team ID is required")]
    public int TeamId { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}
