using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for production management operations
/// </summary>
public interface IProductionService
{
    /// <summary>
    /// Gets all orders assigned to a specific production manager
    /// </summary>
    Task<IEnumerable<OrderDto>> GetOrdersByProductionManagerAsync(int productionManagerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new work task for an order
    /// </summary>
    Task<WorkTaskDto> CreateTaskAsync(CreateWorkTaskDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing work task
    /// </summary>
    Task<WorkTaskDto> UpdateTaskAsync(int id, UpdateWorkTaskDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a task to a specific worker, validates task sequence
    /// </summary>
    Task AssignTaskAsync(int taskId, AssignTaskDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Worker accepts a task that was assigned to them
    /// </summary>
    Task AcceptTaskAsync(int taskId, AcceptTaskDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a task as completed
    /// </summary>
    Task CompleteTaskAsync(int taskId, CompleteTaskDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tasks for a specific team
    /// </summary>
    Task<IEnumerable<WorkTaskDto>> GetTasksByTeamAsync(int teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tasks assigned to a specific worker
    /// </summary>
    Task<IEnumerable<WorkTaskDto>> GetTasksByWorkerAsync(int workerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that task sequence is correct before assignment
    /// Task N+1 cannot be started until Task N is completed
    /// </summary>
    Task<bool> ValidateTaskSequenceAsync(int taskId, CancellationToken cancellationToken = default);
}
