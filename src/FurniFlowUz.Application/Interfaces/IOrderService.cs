using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for order management
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets all orders
    /// </summary>
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an order by ID with all related data
    /// </summary>
    Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated orders with filtering
    /// </summary>
    Task<PaginatedResult<OrderListDto>> GetPagedAsync(OrderFilterDto filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new order with auto-generated order number
    /// </summary>
    Task<OrderDto> CreateAsync(CreateOrderDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order
    /// </summary>
    Task<OrderDto> UpdateAsync(int id, UpdateOrderDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a constructor to an order
    /// </summary>
    Task AssignConstructorAsync(int orderId, int constructorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a production manager to an order
    /// </summary>
    Task AssignProductionManagerAsync(int orderId, int productionManagerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates order status
    /// </summary>
    Task UpdateStatusAsync(int id, OrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an order (soft delete)
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique order number
    /// </summary>
    Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order statistics
    /// </summary>
    Task<OrderStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}
