using FurniFlowUz.Application.DTOs.Auth;
using FurniFlowUz.Application.DTOs.Category;
using FurniFlowUz.Application.DTOs.Constructor;
using FurniFlowUz.Application.DTOs.Contract;
using FurniFlowUz.Application.DTOs.Customer;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// Full order details DTO including furniture types
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Unique order identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Order number (system-generated)
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Associated contract
    /// </summary>
    public ContractSummaryDto? Contract { get; set; }

    /// <summary>
    /// Customer associated with this order
    /// </summary>
    public CustomerDto Customer { get; set; } = null!;

    /// <summary>
    /// Furniture category (primary/first category for backward compatibility)
    /// </summary>
    public CategoryDto Category { get; set; } = null!;

    /// <summary>
    /// All categories associated with this order (Many-to-Many)
    /// </summary>
    public List<CategoryDto> Categories { get; set; } = new();

    /// <summary>
    /// Order description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Expected delivery date
    /// </summary>
    public DateTime ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// Actual delivery date (null if not delivered)
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }

    /// <summary>
    /// Order status (Pending, InProgress, Completed, Cancelled)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Furniture types in this order
    /// </summary>
    public List<FurnitureTypeDto> FurnitureTypes { get; set; } = new();

    /// <summary>
    /// Assigned constructor
    /// </summary>
    public UserDto? AssignedConstructor { get; set; }

    /// <summary>
    /// Assigned production manager
    /// </summary>
    public UserDto? AssignedProductionManager { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User who created the order
    /// </summary>
    public UserDto? CreatedBy { get; set; }

    /// <summary>
    /// Date and time when order was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when order was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Total count of images (room + design) for this order
    /// </summary>
    public int ImagesCount { get; set; }
}
