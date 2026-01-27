using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// DTO for updating an existing order
/// </summary>
public class UpdateOrderDto
{
    /// <summary>
    /// Order description
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Expected delivery date
    /// </summary>
    [Required(ErrorMessage = "Expected delivery date is required")]
    public DateTime ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// Actual delivery date (optional)
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }

    /// <summary>
    /// Order status (Pending, InProgress, Completed, Cancelled)
    /// </summary>
    [Required(ErrorMessage = "Status is required")]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }
}
