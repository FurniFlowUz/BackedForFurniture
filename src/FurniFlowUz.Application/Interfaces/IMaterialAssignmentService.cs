using FurniFlowUz.Application.DTOs.MaterialAssignment;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for material assignment management (Warehouse → Team/Employee)
/// </summary>
public interface IMaterialAssignmentService
{
    /// <summary>
    /// Gets all material assignments
    /// </summary>
    Task<IEnumerable<MaterialAssignmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a material assignment by ID
    /// </summary>
    Task<MaterialAssignmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all assignments for a specific team
    /// </summary>
    Task<IEnumerable<MaterialAssignmentDto>> GetByTeamAsync(int teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all assignments for a specific employee
    /// </summary>
    Task<IEnumerable<MaterialAssignmentDto>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending (unconfirmed) assignments
    /// </summary>
    Task<IEnumerable<MaterialAssignmentDto>> GetPendingAssignmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns material to a team
    /// </summary>
    Task<MaterialAssignmentDto> AssignToTeamAsync(CreateMaterialAssignmentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns material to an employee
    /// </summary>
    Task<MaterialAssignmentDto> AssignToEmployeeAsync(CreateMaterialAssignmentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms receipt of assigned materials
    /// </summary>
    Task<MaterialAssignmentDto> ConfirmReceiptAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a material assignment
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
