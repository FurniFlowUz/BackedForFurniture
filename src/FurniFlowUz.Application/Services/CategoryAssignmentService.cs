using AutoMapper;
using FurniFlowUz.Application.DTOs.CategoryAssignment;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for category assignment management
/// </summary>
public class CategoryAssignmentService : ICategoryAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CategoryAssignmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AssignmentStatsDto> GetAssignmentStatsAsync(CancellationToken cancellationToken = default)
    {
        // Get all assignments with necessary includes
        var assignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            includeProperties: "Order,Order.Customer,FurnitureType,TeamLeader,Team,DetailTasks",
            cancellationToken: cancellationToken);

        var assignmentsList = assignments.ToList();

        // Calculate statistics
        var totalAssignments = assignmentsList.Count;
        var pendingAssignments = assignmentsList.Count(a => a.Status == CategoryAssignmentStatus.Assigned);
        var inProgressAssignments = assignmentsList.Count(a => a.Status == CategoryAssignmentStatus.InProgress);
        var completedAssignments = assignmentsList.Count(a => a.Status == CategoryAssignmentStatus.Completed);
        var onHoldAssignments = assignmentsList.Count(a => a.Status == CategoryAssignmentStatus.OnHold);

        // For overdue, we check if the order has a deadline and it's past
        var overdueAssignments = assignmentsList.Count(a =>
            a.Status != CategoryAssignmentStatus.Completed &&
            a.Order?.DeadlineDate != null &&
            a.Order.DeadlineDate < DateTime.UtcNow);

        // Calculate average completion percentage
        var completionPercentages = new List<decimal>();
        foreach (var assignment in assignmentsList)
        {
            var totalTasks = assignment.DetailTasks?.Count ?? 0;
            if (totalTasks > 0)
            {
                var completedTasks = assignment.DetailTasks?.Count(t =>
                    t.Status == DetailTaskStatus.Completed ||
                    t.Status == DetailTaskStatus.QCPassed) ?? 0;
                completionPercentages.Add((decimal)completedTasks / totalTasks * 100);
            }
        }

        var averageCompletion = completionPercentages.Any()
            ? Math.Round(completionPercentages.Average(), 2)
            : 0;

        // Count active team leaders
        var activeTeamLeaders = assignmentsList
            .Where(a => a.Status != CategoryAssignmentStatus.Completed)
            .Select(a => a.TeamLeaderId)
            .Distinct()
            .Count();

        // Get recent assignments (last 10)
        var recentAssignments = assignmentsList
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .Select(assignment =>
            {
                var dto = _mapper.Map<CategoryAssignmentSummaryDto>(assignment);
                dto.OrderNumber = assignment.Order?.OrderNumber ?? "";
                dto.CustomerName = assignment.Order?.Customer?.FullName ?? "";
                dto.FurnitureTypeName = assignment.FurnitureType?.Name ?? "";
                dto.TeamLeaderName = $"{assignment.TeamLeader?.FirstName} {assignment.TeamLeader?.LastName}".Trim();
                dto.TeamName = assignment.Team?.Name ?? "";

                var totalTasks = assignment.DetailTasks?.Count ?? 0;
                var completedTasks = assignment.DetailTasks?.Count(t =>
                    t.Status == DetailTaskStatus.Completed ||
                    t.Status == DetailTaskStatus.QCPassed) ?? 0;
                dto.TaskProgress = $"{completedTasks}/{totalTasks}";
                dto.CompletionPercent = totalTasks > 0
                    ? Math.Round((decimal)completedTasks / totalTasks * 100, 2)
                    : 0;

                return dto;
            })
            .ToList();

        return new AssignmentStatsDto
        {
            TotalAssignments = totalAssignments,
            PendingAssignments = pendingAssignments,
            InProgressAssignments = inProgressAssignments,
            CompletedAssignments = completedAssignments,
            OverdueAssignments = overdueAssignments,
            AverageCompletionPercentage = averageCompletion,
            ActiveTeamLeaders = activeTeamLeaders,
            RecentAssignments = recentAssignments
        };
    }

    public async Task<IEnumerable<CategoryAssignmentSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            includeProperties: "Order,Order.Customer,FurnitureType,TeamLeader,Team,DetailTasks",
            cancellationToken: cancellationToken);

        var result = new List<CategoryAssignmentSummaryDto>();
        foreach (var assignment in assignments)
        {
            var dto = _mapper.Map<CategoryAssignmentSummaryDto>(assignment);
            dto.OrderNumber = assignment.Order?.OrderNumber ?? "";
            dto.CustomerName = assignment.Order?.Customer?.FullName ?? "";
            dto.FurnitureTypeName = assignment.FurnitureType?.Name ?? "";
            dto.TeamLeaderName = $"{assignment.TeamLeader?.FirstName} {assignment.TeamLeader?.LastName}".Trim();
            dto.TeamName = assignment.Team?.Name ?? "";

            var totalTasks = assignment.DetailTasks?.Count ?? 0;
            var completedTasks = assignment.DetailTasks?.Count(t => t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed) ?? 0;
            dto.TaskProgress = $"{completedTasks}/{totalTasks}";
            dto.CompletionPercent = totalTasks > 0 ? Math.Round((decimal)completedTasks / totalTasks * 100, 2) : 0;

            result.Add(dto);
        }

        return result;
    }

    public async Task<CategoryAssignmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var assignment = (await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: a => a.Id == id,
            includeProperties: "Order,Order.Customer,FurnitureType,TeamLeader,Team,DetailTasks",
            cancellationToken: cancellationToken)).FirstOrDefault();

        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), id);
        }

        var dto = _mapper.Map<CategoryAssignmentDto>(assignment);
        dto.OrderNumber = assignment.Order?.OrderNumber ?? "";
        dto.CustomerName = assignment.Order?.Customer?.FullName ?? "";
        dto.FurnitureTypeName = assignment.FurnitureType?.Name ?? "";
        dto.TeamLeaderName = $"{assignment.TeamLeader?.FirstName} {assignment.TeamLeader?.LastName}".Trim();
        dto.TeamName = assignment.Team?.Name ?? "";
        dto.TotalTasks = assignment.DetailTasks?.Count ?? 0;
        dto.CompletedTasks = assignment.DetailTasks?.Count(t => t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed) ?? 0;

        return dto;
    }

    public async Task<IEnumerable<CategoryAssignmentSummaryDto>> GetByTeamLeaderAsync(int teamLeaderId, CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.TeamLeaderId == teamLeaderId,
            includeProperties: "Order,Order.Customer,FurnitureType,TeamLeader,Team,DetailTasks",
            cancellationToken: cancellationToken);

        var result = new List<CategoryAssignmentSummaryDto>();
        foreach (var assignment in assignments)
        {
            var dto = _mapper.Map<CategoryAssignmentSummaryDto>(assignment);
            dto.OrderNumber = assignment.Order?.OrderNumber ?? "";
            dto.CustomerName = assignment.Order?.Customer?.FullName ?? "";
            dto.FurnitureTypeName = assignment.FurnitureType?.Name ?? "";
            dto.TeamLeaderName = $"{assignment.TeamLeader?.FirstName} {assignment.TeamLeader?.LastName}".Trim();
            dto.TeamName = assignment.Team?.Name ?? "";

            var totalTasks = assignment.DetailTasks?.Count ?? 0;
            var completedTasks = assignment.DetailTasks?.Count(t => t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed) ?? 0;
            dto.TaskProgress = $"{completedTasks}/{totalTasks}";
            dto.CompletionPercent = totalTasks > 0 ? Math.Round((decimal)completedTasks / totalTasks * 100, 2) : 0;

            result.Add(dto);
        }

        return result;
    }

    public async Task<IEnumerable<CategoryAssignmentSummaryDto>> GetByOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.OrderId == orderId,
            includeProperties: "Order,Order.Customer,FurnitureType,TeamLeader,Team,DetailTasks",
            cancellationToken: cancellationToken);

        var result = new List<CategoryAssignmentSummaryDto>();
        foreach (var assignment in assignments)
        {
            var dto = _mapper.Map<CategoryAssignmentSummaryDto>(assignment);
            dto.OrderNumber = assignment.Order?.OrderNumber ?? "";
            dto.CustomerName = assignment.Order?.Customer?.FullName ?? "";
            dto.FurnitureTypeName = assignment.FurnitureType?.Name ?? "";
            dto.TeamLeaderName = $"{assignment.TeamLeader?.FirstName} {assignment.TeamLeader?.LastName}".Trim();
            dto.TeamName = assignment.Team?.Name ?? "";

            var totalTasks = assignment.DetailTasks?.Count ?? 0;
            var completedTasks = assignment.DetailTasks?.Count(t => t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed) ?? 0;
            dto.TaskProgress = $"{completedTasks}/{totalTasks}";
            dto.CompletionPercent = totalTasks > 0 ? Math.Round((decimal)completedTasks / totalTasks * 100, 2) : 0;

            result.Add(dto);
        }

        return result;
    }

    public async Task<CategoryAssignmentDto> CreateAsync(CreateCategoryAssignmentDto request, CancellationToken cancellationToken = default)
    {
        // Validate order exists
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        // Validate furniture type exists
        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(request.FurnitureTypeId, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), request.FurnitureTypeId);
        }

        // Validate team leader exists
        var teamLeader = await _unitOfWork.Users.GetByIdAsync(request.TeamLeaderId, cancellationToken);
        if (teamLeader == null || teamLeader.Role != UserRole.TeamLeader)
        {
            throw new ValidationException("Invalid team leader specified.");
        }

        // Validate team exists
        var team = await _unitOfWork.Teams.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), request.TeamId);
        }

        // Check if this category is already assigned
        var existingAssignment = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: a => a.OrderId == request.OrderId && a.FurnitureTypeId == request.FurnitureTypeId && a.Status != CategoryAssignmentStatus.Completed,
            cancellationToken: cancellationToken);

        if (existingAssignment.Any())
        {
            throw new ValidationException("This category is already assigned to a team leader.");
        }

        var assignment = _mapper.Map<CategoryAssignment>(request);
        assignment.Status = CategoryAssignmentStatus.Assigned;
        assignment.AssignedAt = DateTime.UtcNow;

        await _unitOfWork.CategoryAssignments.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Update order status to Assigned
        if (order.Status == OrderStatus.New)
        {
            order.Status = OrderStatus.Assigned;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await GetByIdAsync(assignment.Id, cancellationToken);
    }

    public async Task<CategoryAssignmentDto> UpdateStatusAsync(int id, UpdateCategoryAssignmentDto request, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.CategoryAssignments.GetByIdAsync(id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), id);
        }

        assignment.Status = request.Status;

        if (request.Status == CategoryAssignmentStatus.InProgress && assignment.StartedAt == null)
        {
            assignment.StartedAt = DateTime.UtcNow;
        }

        if (request.Status == CategoryAssignmentStatus.Completed)
        {
            assignment.CompletedAt = DateTime.UtcNow;
        }

        _unitOfWork.CategoryAssignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<CategoryAssignmentDto> StartAssignmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.CategoryAssignments.GetByIdAsync(id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), id);
        }

        if (assignment.Status != CategoryAssignmentStatus.Assigned)
        {
            throw new ValidationException("Assignment must be in 'Assigned' status to be started.");
        }

        assignment.Status = CategoryAssignmentStatus.InProgress;
        assignment.StartedAt = DateTime.UtcNow;

        _unitOfWork.CategoryAssignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<CategoryAssignmentDto> CompleteAssignmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.CategoryAssignments.GetByIdAsync(id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), id);
        }

        if (assignment.Status == CategoryAssignmentStatus.Completed)
        {
            throw new ValidationException("Assignment is already completed.");
        }

        assignment.Status = CategoryAssignmentStatus.Completed;
        assignment.CompletedAt = DateTime.UtcNow;

        _unitOfWork.CategoryAssignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.CategoryAssignments.GetByIdAsync(id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), id);
        }

        _unitOfWork.CategoryAssignments.Remove(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
