using FurniFlowUz.Application.DTOs.CategoryAssignment;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for category assignment management (Production Manager → Team Leader)
/// </summary>
public interface ICategoryAssignmentService
{
    /// <summary>
    /// Gets assignment statistics for Production Manager dashboard
    /// </summary>
    Task<AssignmentStatsDto> GetAssignmentStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all category assignments
    /// </summary>
    Task<IEnumerable<CategoryAssignmentSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a category assignment by ID with full details
    /// </summary>
    Task<CategoryAssignmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all assignments for a specific team leader
    /// </summary>
    Task<IEnumerable<CategoryAssignmentSummaryDto>> GetByTeamLeaderAsync(int teamLeaderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all assignments for a specific order
    /// </summary>
    Task<IEnumerable<CategoryAssignmentSummaryDto>> GetByOrderAsync(int orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new category assignment (Production Manager assigns category to Team Leader)
    /// </summary>
    Task<CategoryAssignmentDto> CreateAsync(CreateCategoryAssignmentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a simplified category assignment using category name instead of FurnitureTypeId
    /// Auto-resolves FurnitureTypeId from category name and TeamId from team leader
    /// </summary>
    Task<CategoryAssignmentDto> CreateSimpleAsync(SimpleCategoryAssignmentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates category assignment status
    /// </summary>
    Task<CategoryAssignmentDto> UpdateStatusAsync(int id, UpdateCategoryAssignmentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks assignment as started
    /// </summary>
    Task<CategoryAssignmentDto> StartAssignmentAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks assignment as completed
    /// </summary>
    Task<CategoryAssignmentDto> CompleteAssignmentAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category assignment
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
