using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.OrderCategory;

/// <summary>
/// DTO for creating a new OrderCategory
/// </summary>
public class CreateOrderCategoryDto
{
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Category ID is required")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Deadline date for this order-category
    /// </summary>
    public DateTime? DeadlineDate { get; set; }
}
