using AutoMapper;
using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.DTOs.Production;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Data;
using FurniFlowUz.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for production management operations
/// </summary>
public class ProductionService : IProductionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public ProductionService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByProductionManagerAsync(int productionManagerId, CancellationToken cancellationToken = default)
    {
        // Validate production manager exists and has correct role
        var productionManager = await _unitOfWork.Users.GetByIdAsync(productionManagerId, cancellationToken);
        if (productionManager == null)
        {
            throw new NotFoundException(nameof(User), productionManagerId);
        }

        if (productionManager.Role != UserRole.ProductionManager)
        {
            throw new ValidationException("User must have ProductionManager role.");
        }

        // Get all orders assigned to this production manager
        var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<OrderDto>>(orders.OrderByDescending(o => o.CreatedAt));
    }

    public async Task<WorkTaskDto> CreateTaskAsync(CreateWorkTaskDto request, CancellationToken cancellationToken = default)
    {
        // Validate order exists
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        // Validate furniture type if specified
        if (request.FurnitureTypeId.HasValue)
        {
            var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(
                request.FurnitureTypeId.Value,
                cancellationToken);

            if (furnitureType == null)
            {
                throw new NotFoundException(nameof(FurnitureType), request.FurnitureTypeId.Value);
            }

            if (furnitureType.OrderId != request.OrderId)
            {
                throw new ValidationException("Furniture type does not belong to the specified order.");
            }
        }

        // Validate production stage exists
        var productionStage = await _unitOfWork.ProductionStages.GetByIdAsync(
            request.ProductionStageId,
            cancellationToken);

        if (productionStage == null)
        {
            throw new NotFoundException(nameof(ProductionStage), request.ProductionStageId);
        }

        // Validate team exists
        var team = await _unitOfWork.Teams.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), request.TeamId);
        }

        if (!team.IsActive)
        {
            throw new ValidationException("Team is not active.");
        }

        var task = new WorkTask
        {
            Title = request.Title,
            Description = request.Description,
            OrderId = request.OrderId,
            FurnitureTypeId = request.FurnitureTypeId,
            ProductionStageId = request.ProductionStageId,
            TeamId = request.TeamId,
            SequenceOrder = request.SequenceOrder,
            Status = Domain.Enums.TaskStatus.Pending,
            EstimatedHours = request.EstimatedHours,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.WorkTasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to team leader
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Yangi Vazifa Yaratildi",
            Message = $"'{task.Title}' vazifasi jamoangiz uchun yaratildi.",
            Type = NotificationType.TaskAssigned.ToString(),
            UserId = team.TeamLeaderId
        }, cancellationToken);

        return _mapper.Map<WorkTaskDto>(task);
    }

    public async Task<WorkTaskDto> UpdateTaskAsync(int id, UpdateWorkTaskDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.WorkTasks.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(WorkTask), id);
        }

        // Cannot update task if it's completed
        if (task.Status == Domain.Enums.TaskStatus.Completed)
        {
            throw new BusinessException("Cannot update completed task.");
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.EstimatedHours = request.EstimatedHours;
        task.Notes = request.Notes;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WorkTasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkTaskDto>(task);
    }

    public async Task AssignTaskAsync(int taskId, AssignTaskDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.WorkTasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(WorkTask), taskId);
        }

        // Validate task sequence
        var isValid = await ValidateTaskSequenceAsync(taskId, cancellationToken);
        if (!isValid)
        {
            throw new BusinessException("Cannot assign task. Previous tasks in sequence must be completed first.");
        }

        // Validate worker exists and has correct role
        var worker = await _unitOfWork.Users.GetByIdAsync(request.WorkerId.Value, cancellationToken);
        if (worker == null)
        {
            throw new NotFoundException(nameof(User), request.WorkerId.Value);
        }

        if (worker.Role != UserRole.Worker)
        {
            throw new ValidationException("User must have Worker role.");
        }

        if (!worker.IsActive)
        {
            throw new ValidationException("Worker is not active.");
        }

        // Check if worker is member of the task's team
        var team = await _unitOfWork.Teams.GetByIdAsync(task.TeamId, cancellationToken);
        if (!team.Members.Any(m => m.Id == request.WorkerId))
        {
            throw new ValidationException("Worker must be a member of the task's team.");
        }

        // Assign worker to task
        task.AssignedWorkerId = request.WorkerId;
        task.Status = Domain.Enums.TaskStatus.Pending;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WorkTasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to worker
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Vazifa Tayinlandi",
            Message = $"'{task.Title}' vazifasi sizga tayinlandi.",
            Type = NotificationType.TaskAssigned.ToString(),
            UserId = request.WorkerId
        }, cancellationToken);
    }

    public async Task AcceptTaskAsync(int taskId, AcceptTaskDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.WorkTasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(WorkTask), taskId);
        }

        if (!task.AssignedWorkerId.HasValue)
        {
            throw new BusinessException("Task is not assigned to any worker.");
        }

        if (task.Status != Domain.Enums.TaskStatus.Pending)
        {
            throw new BusinessException($"Task is in '{task.Status}' status and cannot be accepted.");
        }

        // Accept task
        task.Status = Domain.Enums.TaskStatus.Accepted;
        task.StartedAt = DateTime.UtcNow;
        task.Notes = request.Notes ?? task.Notes;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WorkTasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to team leader
        var team = await _unitOfWork.Teams.GetByIdAsync(task.TeamId, cancellationToken);
        var message = $"'{task.Title}' vazifasi ishchi tomonidan qabul qilindi.";

        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Vazifa Holati Yangilandi",
            Message = message,
            Type = nameof(NotificationType.TaskCompleted),
            UserId = team.TeamLeaderId
        }, cancellationToken);
    }

    public async Task CompleteTaskAsync(int taskId, CompleteTaskDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.WorkTasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(WorkTask), taskId);
        }

        if (task.Status != Domain.Enums.TaskStatus.Accepted && task.Status != Domain.Enums.TaskStatus.InProgress)
        {
            throw new BusinessException($"Task must be in 'Accepted' or 'InProgress' status to complete. Current status: {task.Status}");
        }

        // Complete task
        task.Status = Domain.Enums.TaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.ActualHours = request.ActualHours;
        task.Notes = request.Notes ?? task.Notes;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WorkTasks.Update(task);

        // Update order progress
        await UpdateOrderProgressAsync(task.OrderId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to team leader
        var team = await _unitOfWork.Teams.GetByIdAsync(task.TeamId, cancellationToken);
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Vazifa Yakunlandi",
            Message = $"'{task.Title}' vazifasi yakunlandi.",
            Type = nameof(NotificationType.TaskCompleted),
            UserId = team.TeamLeaderId
        }, cancellationToken);

        // Send notification to production manager
        var order = await _unitOfWork.Orders.GetByIdAsync(task.OrderId, cancellationToken);
        if (order.AssignedProductionManagerId.HasValue)
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Vazifa Yakunlandi",
                Message = $"'{task.Title}' vazifasi {order.OrderNumber} buyurtma uchun yakunlandi.",
                Type = nameof(NotificationType.TaskCompleted),
                UserId = order.AssignedProductionManagerId.Value
            }, cancellationToken);
        }
    }

    public async Task<IEnumerable<WorkTaskDto>> GetTasksByTeamAsync(int teamId, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), teamId);
        }

        var tasks = await _unitOfWork.WorkTasks.FindAsync(t => t.TeamId == teamId, cancellationToken);
        return _mapper.Map<IEnumerable<WorkTaskDto>>(tasks.OrderBy(t => t.SequenceOrder));
    }

    public async Task<IEnumerable<WorkTaskDto>> GetTasksByWorkerAsync(int workerId, CancellationToken cancellationToken = default)
    {
        var worker = await _unitOfWork.Users.GetByIdAsync(workerId, cancellationToken);
        if (worker == null)
        {
            throw new NotFoundException(nameof(User), workerId);
        }

        var tasks = await _unitOfWork.WorkTasks.FindAsync(
            t => t.AssignedWorkerId == workerId,
            cancellationToken);

        return _mapper.Map<IEnumerable<WorkTaskDto>>(tasks.OrderByDescending(t => t.CreatedAt));
    }

    public async Task<bool> ValidateTaskSequenceAsync(int taskId, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.WorkTasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(WorkTask), taskId);
        }

        // If this is the first task (sequence 1 or no sequence), it can be started
        if (task.SequenceOrder <= 1)
        {
            return true;
        }

        // Get all tasks for the same order and furniture type (if specified)
        var allTasks = await _unitOfWork.WorkTasks.FindAsync(
            t => t.OrderId == task.OrderId &&
                 t.FurnitureTypeId == task.FurnitureTypeId,
            cancellationToken);

        // Check if all previous tasks in sequence are completed
        var previousTasks = allTasks.Where(t => t.SequenceOrder < task.SequenceOrder);

        foreach (var prevTask in previousTasks)
        {
            if (prevTask.Status != Domain.Enums.TaskStatus.Completed)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<IEnumerable<ProductionStageDto>> GetAllStagesAsync(CancellationToken cancellationToken = default)
    {
        var stages = await _unitOfWork.ProductionStages.GetAllAsync(cancellationToken);
        var activeStages = stages.Where(s => s.IsActive).OrderBy(s => s.SequenceOrder);
        return _mapper.Map<IEnumerable<ProductionStageDto>>(activeStages);
    }

    public async Task<IEnumerable<WorkTaskDto>> GetTasksByAssignmentAsync(int assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.CategoryAssignments.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), assignmentId);
        }

        // Get all tasks for this order with includes
        var tasks = await _unitOfWork.WorkTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1000,
            filter: t => t.OrderId == assignment.OrderId,
            orderBy: q => q.OrderBy(t => t.SequenceOrder),
            includeProperties: "Order,ProductionStage,Team,AssignedWorker,FurnitureType",
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<WorkTaskDto>>(tasks);
    }

    public async Task<IEnumerable<WorkTaskDto>> GetTasksByOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        // Get all tasks for this order with includes
        var tasks = await _unitOfWork.WorkTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1000,
            filter: t => t.OrderId == orderId,
            orderBy: q => q.OrderBy(t => t.SequenceOrder),
            includeProperties: "Order,ProductionStage,Team,AssignedWorker,FurnitureType",
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<WorkTaskDto>>(tasks);
    }

    public async Task StartTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.WorkTasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(WorkTask), taskId);
        }

        if (task.Status != Domain.Enums.TaskStatus.Pending && task.Status != Domain.Enums.TaskStatus.Accepted)
        {
            throw new BusinessException($"Task must be in 'Pending' or 'Accepted' status to start. Current status: {task.Status}");
        }

        task.Status = Domain.Enums.TaskStatus.InProgress;
        task.StartedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WorkTasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification
        var team = await _unitOfWork.Teams.GetByIdAsync(task.TeamId, cancellationToken);
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Vazifa Boshlandi",
            Message = $"'{task.Title}' vazifasi boshlandi.",
            Type = nameof(NotificationType.TaskAssigned),
            UserId = team.TeamLeaderId
        }, cancellationToken);
    }

    public async Task CreateTasksForAssignmentAsync(int assignmentId, int orderId, int teamId, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.CategoryAssignments.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(CategoryAssignment), assignmentId);
        }

        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        var team = await _unitOfWork.Teams.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), teamId);
        }

        // Get all production stages
        var stages = await _unitOfWork.ProductionStages.GetAllAsync(cancellationToken);
        var activeStages = stages.Where(s => s.IsActive).OrderBy(s => s.SequenceOrder).ToList();

        // Check if tasks already exist for this order
        var existingTasks = await _unitOfWork.WorkTasks.FindAsync(t => t.OrderId == orderId, cancellationToken);
        if (existingTasks.Any())
        {
            // Tasks already created for this order
            return;
        }

        // Create a task for each stage
        foreach (var stage in activeStages)
        {
            var newTask = new WorkTask
            {
                Title = stage.Name,
                Description = stage.Description,
                OrderId = orderId,
                ProductionStageId = stage.Id,
                TeamId = teamId,
                SequenceOrder = stage.SequenceOrder,
                Status = Domain.Enums.TaskStatus.Pending,
                EstimatedHours = stage.EstimatedDurationHours,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.WorkTasks.AddAsync(newTask, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to team leader
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Yangi Vazifalar Yaratildi",
            Message = $"{order.OrderNumber} buyurtmasi uchun {activeStages.Count} ta vazifa yaratildi.",
            Type = NotificationType.TaskAssigned.ToString(),
            UserId = team.TeamLeaderId
        }, cancellationToken);
    }

    #region Private Helper Methods

    private async Task UpdateOrderProgressAsync(int orderId, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            return;
        }

        // Get all tasks for this order
        var allTasks = await _unitOfWork.WorkTasks.FindAsync(t => t.OrderId == orderId, cancellationToken);

        if (!allTasks.Any())
        {
            return;
        }

        // Calculate progress percentage
        var completedTasks = allTasks.Count(t => t.Status == Domain.Enums.TaskStatus.Completed);
        var totalTasks = allTasks.Count();

        var progressPercentage = (decimal)completedTasks / totalTasks * 100;

        order.ProgressPercentage = Math.Round(progressPercentage, 2);

        // Update order status based on progress
        if (progressPercentage > 0 && progressPercentage < 100 && order.Status == OrderStatus.SpecificationsReady)
        {
            order.Status = OrderStatus.InProduction;
        }
        else if (progressPercentage == 100)
        {
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;
        }

        order.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Orders.Update(order);
    }

    #endregion
}
