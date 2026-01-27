using AutoMapper;
using FurniFlowUz.Application.DTOs.DetailTask;
using FurniFlowUz.Application.DTOs.TaskPerformance;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for detail task management (Team Leader → Employee tasks)
/// </summary>
public class DetailTaskService : IDetailTaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITaskPerformanceService _performanceService;

    public DetailTaskService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ITaskPerformanceService performanceService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _performanceService = performanceService;
    }

    public async Task<DetailTaskDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = (await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: t => t.Id == id,
            includeProperties: "CategoryAssignment,CategoryAssignment.Order,CategoryAssignment.Order.Customer,CategoryAssignment.FurnitureType,Detail,AssignedEmployee,DependsOnTask",
            cancellationToken: cancellationToken)).FirstOrDefault();

        if (task == null)
        {
            throw new NotFoundException(nameof(DetailTask), id);
        }

        return MapToDetailTaskDto(task);
    }

    public async Task<IEnumerable<DetailTaskListDto>> GetByCategoryAssignmentAsync(int categoryAssignmentId, CancellationToken cancellationToken = default)
    {
        var tasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.CategoryAssignmentId == categoryAssignmentId,
            includeProperties: "Detail,AssignedEmployee,DependsOnTask",
            orderBy: q => q.OrderBy(t => t.Sequence),
            cancellationToken: cancellationToken);

        var result = new List<DetailTaskListDto>();
        foreach (var task in tasks)
        {
            var dto = _mapper.Map<DetailTaskListDto>(task);
            dto.DetailName = task.Detail?.Name ?? "";
            dto.AssignedEmployeeName = task.AssignedEmployee != null
                ? $"{task.AssignedEmployee.FirstName} {task.AssignedEmployee.LastName}".Trim()
                : "";
            dto.IsLocked = await IsTaskLockedAsync(task, cancellationToken);
            result.Add(dto);
        }

        return result;
    }

    public async Task<EmployeeTaskQueueDto> GetMyTaskQueueAsync(CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        return await GetEmployeeTaskQueueAsync(_currentUserService.UserId.Value, cancellationToken);
    }

    public async Task<EmployeeTaskQueueDto> GetEmployeeTaskQueueAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var tasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.AssignedEmployeeId == employeeId &&
                        (t.Status == DetailTaskStatus.Pending ||
                         t.Status == DetailTaskStatus.Ready ||
                         t.Status == DetailTaskStatus.InProgress),
            includeProperties: "CategoryAssignment,CategoryAssignment.Order,CategoryAssignment.Order.Customer,CategoryAssignment.FurnitureType,Detail,DependsOnTask",
            orderBy: q => q.OrderBy(t => t.Sequence),
            cancellationToken: cancellationToken);

        var queue = new EmployeeTaskQueueDto
        {
            TotalTasks = tasks.Count(),
            InProgressTasks = tasks.Count(t => t.Status == DetailTaskStatus.InProgress)
        };

        // Current task: First InProgress task, or first Ready task
        var currentTask = tasks.FirstOrDefault(t => t.Status == DetailTaskStatus.InProgress)
                       ?? tasks.FirstOrDefault(t => t.Status == DetailTaskStatus.Ready);

        if (currentTask != null)
        {
            queue.CurrentTask = MapToDetailTaskDto(currentTask);
        }

        // Upcoming tasks: Ready tasks (excluding current)
        var upcomingTasks = tasks.Where(t => t.Status == DetailTaskStatus.Ready && t.Id != currentTask?.Id);
        foreach (var task in upcomingTasks)
        {
            var dto = _mapper.Map<DetailTaskListDto>(task);
            dto.DetailName = task.Detail?.Name ?? "";
            dto.AssignedEmployeeName = $"{task.AssignedEmployee?.FirstName} {task.AssignedEmployee?.LastName}".Trim();
            dto.IsLocked = false;
            queue.UpcomingTasks.Add(dto);
        }

        // Locked tasks: Pending tasks (waiting for dependencies)
        var lockedTasks = tasks.Where(t => t.Status == DetailTaskStatus.Pending);
        foreach (var task in lockedTasks)
        {
            var dto = _mapper.Map<DetailTaskListDto>(task);
            dto.DetailName = task.Detail?.Name ?? "";
            dto.AssignedEmployeeName = $"{task.AssignedEmployee?.FirstName} {task.AssignedEmployee?.LastName}".Trim();
            dto.IsLocked = true;
            queue.LockedTasks.Add(dto);
        }

        // Get completed tasks count
        var completedTasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.AssignedEmployeeId == employeeId &&
                        (t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed),
            cancellationToken: cancellationToken);
        queue.CompletedTasks = completedTasks.Count();

        return queue;
    }

    public async Task<DetailTaskDto?> GetNextTaskAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var tasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: t => t.AssignedEmployeeId == employeeId && t.Status == DetailTaskStatus.Ready,
            includeProperties: "CategoryAssignment,CategoryAssignment.Order,CategoryAssignment.Order.Customer,CategoryAssignment.FurnitureType,Detail,AssignedEmployee",
            orderBy: q => q.OrderBy(t => t.Sequence),
            cancellationToken: cancellationToken);

        var nextTask = tasks.FirstOrDefault();
        return nextTask != null ? MapToDetailTaskDto(nextTask) : null;
    }

    public async Task<DetailTaskDto> CreateAsync(CreateDetailTaskDto request, CancellationToken cancellationToken = default)
    {
        // Validate category assignment exists
        var assignment = await _unitOfWork.CategoryAssignments.GetByIdAsync(request.CategoryAssignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), request.CategoryAssignmentId);
        }

        // Validate detail exists
        var detail = await _unitOfWork.Details.GetByIdAsync(request.DetailId, cancellationToken);
        if (detail == null)
        {
            throw new NotFoundException(nameof(Detail), request.DetailId);
        }

        // Validate employee exists and is active
        var employee = await _unitOfWork.Users.GetByIdAsync(request.AssignedEmployeeId, cancellationToken);
        if (employee == null || !employee.IsActive)
        {
            throw new ValidationException("Invalid or inactive employee specified.");
        }

        // Validate depends on task if specified
        if (request.DependsOnTaskId.HasValue)
        {
            var dependsOnTask = await _unitOfWork.DetailTasks.GetByIdAsync(request.DependsOnTaskId.Value, cancellationToken);
            if (dependsOnTask == null)
            {
                throw new NotFoundException(nameof(DetailTask), request.DependsOnTaskId.Value);
            }

            // Ensure dependency is in same category assignment
            if (dependsOnTask.CategoryAssignmentId != request.CategoryAssignmentId)
            {
                throw new ValidationException("Dependent task must be in the same category assignment.");
            }
        }

        var task = _mapper.Map<DetailTask>(request);

        // Determine initial status based on dependencies
        if (request.DependsOnTaskId.HasValue)
        {
            var dependentTask = await _unitOfWork.DetailTasks.GetByIdAsync(request.DependsOnTaskId.Value, cancellationToken);
            task.Status = (dependentTask!.Status == DetailTaskStatus.Completed || dependentTask.Status == DetailTaskStatus.QCPassed)
                ? DetailTaskStatus.Ready
                : DetailTaskStatus.Pending;
        }
        else
        {
            task.Status = DetailTaskStatus.Ready; // No dependencies, ready to start
        }

        await _unitOfWork.DetailTasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(task.Id, cancellationToken);
    }

    public async Task<DetailTaskDto> UpdateStatusAsync(int id, UpdateDetailTaskDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.DetailTasks.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(DetailTask), id);
        }

        task.Status = request.Status;
        _unitOfWork.DetailTasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Check if any dependent tasks should be unlocked
        await UnlockDependentTasksAsync(id, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<DetailTaskDto> StartTaskAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.DetailTasks.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(DetailTask), id);
        }

        if (task.Status != DetailTaskStatus.Ready)
        {
            throw new ValidationException("Task must be in 'Ready' status to be started.");
        }

        // Check if dependency is completed
        if (task.DependsOnTaskId.HasValue)
        {
            var isLocked = await IsTaskLockedAsync(task, cancellationToken);
            if (isLocked)
            {
                throw new ValidationException("Cannot start task. Dependent task is not completed.");
            }
        }

        task.Status = DetailTaskStatus.InProgress;
        task.StartedAt = DateTime.UtcNow;

        _unitOfWork.DetailTasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<DetailTaskDto> CompleteTaskAsync(int id, CreateTaskPerformanceDto performanceData, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.DetailTasks.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(DetailTask), id);
        }

        if (task.Status != DetailTaskStatus.InProgress)
        {
            throw new ValidationException("Task must be in 'InProgress' status to be completed.");
        }

        task.Status = DetailTaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;

        _unitOfWork.DetailTasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Record performance
        performanceData.DetailTaskId = id;
        await _performanceService.RecordPerformanceAsync(performanceData, cancellationToken);

        // Unlock dependent tasks
        await UnlockDependentTasksAsync(id, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.DetailTasks.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(DetailTask), id);
        }

        _unitOfWork.DetailTasks.Remove(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // Helper methods

    private async Task<bool> IsTaskLockedAsync(DetailTask task, CancellationToken cancellationToken)
    {
        if (!task.DependsOnTaskId.HasValue)
        {
            return false; // No dependency, not locked
        }

        var dependentTask = await _unitOfWork.DetailTasks.GetByIdAsync(task.DependsOnTaskId.Value, cancellationToken);
        if (dependentTask == null)
        {
            return false;
        }

        // Task is locked if dependent task is not completed or QC passed
        return dependentTask.Status != DetailTaskStatus.Completed &&
               dependentTask.Status != DetailTaskStatus.QCPassed;
    }

    private async Task UnlockDependentTasksAsync(int completedTaskId, CancellationToken cancellationToken)
    {
        // Find all tasks that depend on this completed task
        var dependentTasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.DependsOnTaskId == completedTaskId && t.Status == DetailTaskStatus.Pending,
            cancellationToken: cancellationToken);

        foreach (var task in dependentTasks)
        {
            task.Status = DetailTaskStatus.Ready;
            _unitOfWork.DetailTasks.Update(task);
        }

        if (dependentTasks.Any())
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private DetailTaskDto MapToDetailTaskDto(DetailTask task)
    {
        var dto = _mapper.Map<DetailTaskDto>(task);
        dto.DetailName = task.Detail?.Name ?? "";
        dto.AssignedEmployeeName = task.AssignedEmployee != null
            ? $"{task.AssignedEmployee.FirstName} {task.AssignedEmployee.LastName}".Trim()
            : "";
        dto.DependsOnTaskName = task.DependsOnTask?.TaskDescription ?? "";
        dto.OrderNumber = task.CategoryAssignment?.Order?.OrderNumber ?? "";
        dto.CustomerName = task.CategoryAssignment?.Order?.Customer?.FullName ?? "";
        dto.FurnitureTypeName = task.CategoryAssignment?.FurnitureType?.Name ?? "";
        dto.StartedAt = task.StartedAt;
        dto.CompletedAt = task.CompletedAt;
        return dto;
    }
}
