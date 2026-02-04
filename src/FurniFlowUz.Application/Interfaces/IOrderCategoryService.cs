using FurniFlowUz.Application.DTOs.OrderCategory;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for managing Order-Category relationships
/// </summary>
public interface IOrderCategoryService
{
    /// <summary>
    /// Gets all categories for a specific order
    /// </summary>
    Task<IEnumerable<OrderCategoryDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders for a specific category
    /// </summary>
    Task<IEnumerable<OrderCategoryDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific order-category relationship by ID
    /// </summary>
    Task<OrderCategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a category to an order
    /// </summary>
    Task<OrderCategoryDto> AddCategoryToOrderAsync(CreateOrderCategoryDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a category from an order
    /// </summary>
    Task RemoveCategoryFromOrderAsync(int orderId, int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets all categories for an order (replaces existing)
    /// </summary>
    Task<IEnumerable<OrderCategoryDto>> SetOrderCategoriesAsync(BulkOrderCategoryDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an order-category relationship by ID
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
