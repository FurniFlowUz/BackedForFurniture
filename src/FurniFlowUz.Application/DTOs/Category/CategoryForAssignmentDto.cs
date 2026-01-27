namespace FurniFlowUz.Application.DTOs.Category;

/// <summary>
/// DTO for categories available for assignment to team leaders
/// </summary>
public class CategoryForAssignmentDto
{
    /// <summary>
    /// Furniture type identifier (used as category ID in this context)
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Furniture type name (displayed as category name)
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Order identifier
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Order number for display
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer full name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
}
