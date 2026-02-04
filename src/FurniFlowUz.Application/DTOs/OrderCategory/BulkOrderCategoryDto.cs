using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.OrderCategory;

/// <summary>
/// DTO for bulk adding/updating categories for an order
/// </summary>
public class BulkOrderCategoryDto
{
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "At least one category is required")]
    [MinLength(1, ErrorMessage = "At least one category is required")]
    public List<int> CategoryIds { get; set; } = new();
}
