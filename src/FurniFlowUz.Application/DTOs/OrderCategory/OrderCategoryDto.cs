using FurniFlowUz.Application.DTOs.Category;

namespace FurniFlowUz.Application.DTOs.OrderCategory;

/// <summary>
/// DTO for OrderCategory response
/// </summary>
public class OrderCategoryDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public CategoryDto? Category { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
