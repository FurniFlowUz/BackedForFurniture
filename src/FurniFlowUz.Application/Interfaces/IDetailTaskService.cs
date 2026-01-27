using FurniFlowUz.Application.DTOs.DetailTask;
using FurniFlowUz.Application.DTOs.TaskPerformance;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for detail task management (Team Leader → Employee tasks)
/// </summary>
public interface IDetailTaskService
{
    /// <summary>
    /// Gets a task by ID with full details
    /// </summary>
    Task<DetailTaskDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tasks for a specific category assignment (Team Leader view)
    /// </summary>
    Task<IEnumerable<DetailTaskListDto>> GetByCategoryAssignmentAsync(int categoryAssignmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task queue for current employee (Employee view)
    /// </summary>
    Task<EmployeeTaskQueueDto> GetMyTaskQueueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets task queue for a specific employee
    /// </summary>
    Task<EmployeeTaskQueueDto> GetEmployeeTaskQueueAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next available task for an employee (sequential logic)
    /// </summary>
    Task<DetailTaskDto?> GetNextTaskAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new detail task (Team Leader assigns task to Employee)
    /// </summary>
    Task<DetailTaskDto> CreateAsync(CreateDetailTaskDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates task status
    /// </summary>
    Task<DetailTaskDto> UpdateStatusAsync(int id, UpdateDetailTaskDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a task (Employee begins work)
    /// </summary>
    Task<DetailTaskDto> StartTaskAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a task and records performance
    /// </summary>
    Task<DetailTaskDto> CompleteTaskAsync(int id, CreateTaskPerformanceDto performanceData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a task
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
