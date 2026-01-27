using AutoMapper;
using FurniFlowUz.Application.DTOs.MaterialAssignment;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for material assignment management (Warehouse → Team/Employee)
/// </summary>
public class MaterialAssignmentService : IMaterialAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public MaterialAssignmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<MaterialAssignmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.MaterialAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            includeProperties: "MaterialRequest,WarehouseItem,AssignedToTeam,AssignedToEmployee",
            cancellationToken: cancellationToken);

        return MapToMaterialAssignmentDtos(assignments);
    }

    public async Task<MaterialAssignmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var assignment = (await _unitOfWork.MaterialAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: a => a.Id == id,
            includeProperties: "MaterialRequest,WarehouseItem,AssignedToTeam,AssignedToEmployee",
            cancellationToken: cancellationToken)).FirstOrDefault();

        if (assignment == null)
        {
            throw new NotFoundException(nameof(MaterialAssignment), id);
        }

        return MapToMaterialAssignmentDto(assignment);
    }

    public async Task<IEnumerable<MaterialAssignmentDto>> GetByTeamAsync(int teamId, CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.MaterialAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.AssignedToTeamId == teamId,
            includeProperties: "MaterialRequest,WarehouseItem,AssignedToTeam,AssignedToEmployee",
            cancellationToken: cancellationToken);

        return MapToMaterialAssignmentDtos(assignments);
    }

    public async Task<IEnumerable<MaterialAssignmentDto>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.MaterialAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.AssignedToEmployeeId == employeeId,
            includeProperties: "MaterialRequest,WarehouseItem,AssignedToTeam,AssignedToEmployee",
            cancellationToken: cancellationToken);

        return MapToMaterialAssignmentDtos(assignments);
    }

    public async Task<IEnumerable<MaterialAssignmentDto>> GetPendingAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.MaterialAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => !a.ReceivedConfirmed,
            includeProperties: "MaterialRequest,WarehouseItem,AssignedToTeam,AssignedToEmployee",
            cancellationToken: cancellationToken);

        return MapToMaterialAssignmentDtos(assignments);
    }

    public async Task<MaterialAssignmentDto> AssignToTeamAsync(CreateMaterialAssignmentDto request, CancellationToken cancellationToken = default)
    {
        // Validate team assignment
        if (!request.AssignedToTeamId.HasValue)
        {
            throw new ValidationException("Team ID is required for team assignment.");
        }

        if (request.AssignedToEmployeeId.HasValue)
        {
            throw new ValidationException("Cannot assign to both team and employee. Choose one.");
        }

        // Validate material request exists
        var materialRequest = await _unitOfWork.MaterialRequests.GetByIdAsync(request.MaterialRequestId, cancellationToken);
        if (materialRequest == null)
        {
            throw new NotFoundException(nameof(MaterialRequest), request.MaterialRequestId);
        }

        // Validate warehouse item exists
        var warehouseItem = await _unitOfWork.WarehouseItems.GetByIdAsync(request.WarehouseItemId, cancellationToken);
        if (warehouseItem == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), request.WarehouseItemId);
        }

        // Validate team exists
        var team = await _unitOfWork.Teams.GetByIdAsync(request.AssignedToTeamId.Value, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), request.AssignedToTeamId.Value);
        }

        // Check stock availability
        if (warehouseItem.CurrentStock < request.Quantity)
        {
            throw new ValidationException($"Insufficient stock. Available: {warehouseItem.CurrentStock}, Requested: {request.Quantity}");
        }

        var assignment = _mapper.Map<MaterialAssignment>(request);
        assignment.ReceivedConfirmed = false;

        await _unitOfWork.MaterialAssignments.AddAsync(assignment, cancellationToken);

        // Update warehouse stock
        warehouseItem.CurrentStock -= request.Quantity;
        _unitOfWork.WarehouseItems.Update(warehouseItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(assignment.Id, cancellationToken);
    }

    public async Task<MaterialAssignmentDto> AssignToEmployeeAsync(CreateMaterialAssignmentDto request, CancellationToken cancellationToken = default)
    {
        // Validate employee assignment
        if (!request.AssignedToEmployeeId.HasValue)
        {
            throw new ValidationException("Employee ID is required for employee assignment.");
        }

        if (request.AssignedToTeamId.HasValue)
        {
            throw new ValidationException("Cannot assign to both team and employee. Choose one.");
        }

        // Validate material request exists
        var materialRequest = await _unitOfWork.MaterialRequests.GetByIdAsync(request.MaterialRequestId, cancellationToken);
        if (materialRequest == null)
        {
            throw new NotFoundException(nameof(MaterialRequest), request.MaterialRequestId);
        }

        // Validate warehouse item exists
        var warehouseItem = await _unitOfWork.WarehouseItems.GetByIdAsync(request.WarehouseItemId, cancellationToken);
        if (warehouseItem == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), request.WarehouseItemId);
        }

        // Validate employee exists
        var employee = await _unitOfWork.Users.GetByIdAsync(request.AssignedToEmployeeId.Value, cancellationToken);
        if (employee == null || !employee.IsActive)
        {
            throw new ValidationException("Invalid or inactive employee specified.");
        }

        // Check stock availability
        if (warehouseItem.CurrentStock < request.Quantity)
        {
            throw new ValidationException($"Insufficient stock. Available: {warehouseItem.CurrentStock}, Requested: {request.Quantity}");
        }

        var assignment = _mapper.Map<MaterialAssignment>(request);
        assignment.ReceivedConfirmed = false;

        await _unitOfWork.MaterialAssignments.AddAsync(assignment, cancellationToken);

        // Update warehouse stock
        warehouseItem.CurrentStock -= request.Quantity;
        _unitOfWork.WarehouseItems.Update(warehouseItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(assignment.Id, cancellationToken);
    }

    public async Task<MaterialAssignmentDto> ConfirmReceiptAsync(int id, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.MaterialAssignments.GetByIdAsync(id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(MaterialAssignment), id);
        }

        if (assignment.ReceivedConfirmed)
        {
            throw new ValidationException("Receipt already confirmed for this assignment.");
        }

        assignment.ReceivedConfirmed = true;
        assignment.ReceivedAt = DateTime.UtcNow;

        _unitOfWork.MaterialAssignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.MaterialAssignments.GetByIdAsync(id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(MaterialAssignment), id);
        }

        // If not yet confirmed, return stock to warehouse
        if (!assignment.ReceivedConfirmed)
        {
            var warehouseItem = await _unitOfWork.WarehouseItems.GetByIdAsync(assignment.WarehouseItemId, cancellationToken);
            if (warehouseItem != null)
            {
                warehouseItem.CurrentStock += assignment.Quantity;
                _unitOfWork.WarehouseItems.Update(warehouseItem);
            }
        }

        _unitOfWork.MaterialAssignments.Remove(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // Helper methods
    private MaterialAssignmentDto MapToMaterialAssignmentDto(MaterialAssignment assignment)
    {
        var dto = _mapper.Map<MaterialAssignmentDto>(assignment);
        dto.RequestNumber = assignment.MaterialRequest?.Id.ToString() ?? "";
        dto.MaterialName = assignment.WarehouseItem?.Name ?? "";
        dto.MaterialSKU = assignment.WarehouseItem?.SKU ?? "";
        dto.Unit = assignment.WarehouseItem?.Unit.ToString() ?? "";
        dto.TeamName = assignment.AssignedToTeam?.Name;
        dto.EmployeeName = assignment.AssignedToEmployee != null
            ? $"{assignment.AssignedToEmployee.FirstName} {assignment.AssignedToEmployee.LastName}".Trim()
            : null;
        return dto;
    }

    private IEnumerable<MaterialAssignmentDto> MapToMaterialAssignmentDtos(IEnumerable<MaterialAssignment> assignments)
    {
        return assignments.Select(MapToMaterialAssignmentDto).ToList();
    }
}
