using FurniFlowUz.Application.DTOs.Warehouse;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for warehouse and inventory management
/// </summary>
public interface IWarehouseService
{
    /// <summary>
    /// Gets all warehouse items
    /// </summary>
    Task<IEnumerable<WarehouseItemDto>> GetAllItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a warehouse item by ID
    /// </summary>
    Task<WarehouseItemDto> GetItemByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new warehouse item
    /// </summary>
    Task<WarehouseItemDto> CreateItemAsync(CreateWarehouseItemDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing warehouse item
    /// </summary>
    Task<WarehouseItemDto> UpdateItemAsync(int id, UpdateWarehouseItemDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a warehouse item
    /// </summary>
    Task DeleteItemAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an income transaction (stock increase)
    /// </summary>
    Task<WarehouseTransactionDto> CreateIncomeTransactionAsync(CreateIncomeTransactionDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an outcome transaction (stock decrease) and material request for team confirmation
    /// </summary>
    Task<WarehouseTransactionDto> CreateOutcomeTransactionAsync(CreateOutcomeTransactionDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets warehouse items with low stock (CurrentStock less than MinimumStock)
    /// </summary>
    Task<IEnumerable<WarehouseAlertDto>> GetLowStockAlertsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets transaction history for a specific warehouse item
    /// </summary>
    Task<IEnumerable<WarehouseTransactionDto>> GetStockHistoryAsync(int itemId, CancellationToken cancellationToken = default);
}
