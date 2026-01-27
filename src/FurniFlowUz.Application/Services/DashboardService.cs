using AutoMapper;
using FurniFlowUz.Application.DTOs.Dashboard;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for dashboard statistics and analytics with role-based data isolation
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWarehouseService _warehouseService;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IWarehouseService warehouseService,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _warehouseService = warehouseService;
        _currentUserService = currentUserService;
    }

    public async Task<DirectorDashboardDto> GetDirectorDashboardAsync(CancellationToken cancellationToken = default)
    {
        var revenueStats = await GetRevenueStatisticsAsync(cancellationToken);
        var delayedTasks = await GetDelayedTasksAsync(cancellationToken);
        var lowStockAlerts = await _warehouseService.GetLowStockAlertsAsync(cancellationToken);

        // Calculate statistics directly
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var allUsers = await _unitOfWork.Users.GetAllAsync(cancellationToken);

        return new DirectorDashboardDto
        {
            TotalOrders = allOrders.Count(),
            OrdersInProgress = allOrders.Count(o => o.Status == OrderStatus.InProduction || o.Status == OrderStatus.SpecificationsReady),
            DelayedOrders = allOrders.Count(o => o.DeadlineDate < DateTime.UtcNow && o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled),
            CompletedOrders = allOrders.Count(o => o.Status == OrderStatus.Completed),
            TotalRevenue = revenueStats.TotalRevenue,
            DailyRevenue = revenueStats.DailyRevenue,
            MonthlyRevenue = revenueStats.MonthlyRevenue,
            ActiveWorkers = allUsers.Count(u => u.Role == UserRole.Worker && u.IsActive),
            WarehouseAlerts = lowStockAlerts.ToList(),
            DelayedTasks = delayedTasks.ToList()
        };
    }

    public async Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

        // ✅ Apply role-based filtering
        var filteredOrders = await ApplyRoleBasedOrderFilteringAsync(allOrders, cancellationToken);

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        var previousMonth = DateTime.UtcNow.AddMonths(-1).Month;
        var previousYear = DateTime.UtcNow.AddMonths(-1).Year;

        var currentPeriodCount = filteredOrders.Count(o => o.CreatedAt.Month == currentMonth && o.CreatedAt.Year == currentYear);
        var previousPeriodCount = filteredOrders.Count(o => o.CreatedAt.Month == previousMonth && o.CreatedAt.Year == previousYear);
        var percentageChange = previousPeriodCount > 0 ? ((decimal)(currentPeriodCount - previousPeriodCount) / previousPeriodCount) * 100 : 0;

        return new StatisticsDto
        {
            TotalCount = filteredOrders.Count(),
            CurrentPeriodCount = currentPeriodCount,
            PreviousPeriodCount = previousPeriodCount,
            PercentageChange = percentageChange
        };
    }

    public async Task<RevenueStatisticsDto> GetRevenueStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var allContracts = await _unitOfWork.Contracts.GetAllAsync(cancellationToken);

        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var dailyRevenue = allContracts
            .Where(c => c.CreatedAt.Date == today && c.Status != ContractStatus.Cancelled)
            .Sum(c => c.TotalAmount);

        var monthlyRevenue = allContracts
            .Where(c => c.CreatedAt >= startOfMonth && c.CreatedAt < endOfMonth && c.Status != ContractStatus.Cancelled)
            .Sum(c => c.TotalAmount);

        var totalRevenue = allContracts
            .Where(c => c.Status != ContractStatus.Cancelled)
            .Sum(c => c.TotalAmount);

        var pendingPayments = allContracts
            .Where(c => c.PaymentStatus == PaymentStatus.Pending || c.PaymentStatus == PaymentStatus.PartiallyPaid)
            .Sum(c => c.RemainingAmount);

        var completedPayments = allContracts
            .Where(c => c.PaymentStatus == PaymentStatus.FullyPaid)
            .Sum(c => c.TotalAmount);

        return new RevenueStatisticsDto
        {
            DailyRevenue = dailyRevenue,
            MonthlyRevenue = monthlyRevenue,
            TotalRevenue = totalRevenue
        };
    }

    public async Task<ProductionStatisticsDto> GetProductionStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var allTasks = await _unitOfWork.WorkTasks.GetAllAsync(cancellationToken);
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

        // ✅ Apply role-based filtering to orders
        var filteredOrders = await ApplyRoleBasedOrderFilteringAsync(allOrders, cancellationToken);

        // ✅ Filter tasks to only those related to accessible orders
        var accessibleOrderIds = filteredOrders.Select(o => o.Id).ToHashSet();
        var filteredTasks = allTasks.Where(t => accessibleOrderIds.Contains(t.OrderId)).ToList();

        var completedTasks = filteredTasks.Where(t => t.Status == Domain.Enums.TaskStatus.Completed).ToList();
        var activeTasks = filteredTasks.Where(t => t.Status == Domain.Enums.TaskStatus.Accepted || t.Status == Domain.Enums.TaskStatus.InProgress).ToList();

        var totalEstimatedHours = filteredTasks.Where(t => t.EstimatedHours.HasValue).Sum(t => t.EstimatedHours.Value);
        var totalActualHours = completedTasks.Where(t => t.ActualHours.HasValue).Sum(t => t.ActualHours.Value);

        var efficiency = totalEstimatedHours > 0 ? (totalEstimatedHours / (totalActualHours > 0 ? totalActualHours : 1)) * 100 : 0;

        var ordersInProduction = filteredOrders.Count(o => o.Status == OrderStatus.InProduction);
        var averageProgress = ordersInProduction > 0
            ? filteredOrders.Where(o => o.Status == OrderStatus.InProduction).Average(o => o.ProgressPercentage)
            : 0;

        var delayedTasksCount = filteredOrders.Count(o => o.DeadlineDate < DateTime.UtcNow && o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled);

        return new ProductionStatisticsDto
        {
            TotalTasks = filteredTasks.Count,
            CompletedTasks = completedTasks.Count,
            InProgressTasks = activeTasks.Count,
            PendingTasks = filteredTasks.Count(t => t.Status == Domain.Enums.TaskStatus.Pending),
            DelayedTasks = delayedTasksCount,
            AverageCompletionTime = completedTasks.Any() && completedTasks.Any(t => t.ActualHours.HasValue)
                ? completedTasks.Where(t => t.ActualHours.HasValue).Average(t => t.ActualHours!.Value)
                : 0,
            EfficiencyPercentage = Math.Round(efficiency, 2),
            ActiveTeams = 0, // TODO: Implement team tracking
            ActiveWorkers = 0 // TODO: Implement worker tracking
        };
    }

    public async Task<IEnumerable<DelayedTaskDto>> GetDelayedTasksAsync(CancellationToken cancellationToken = default)
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var allTasks = await _unitOfWork.WorkTasks.GetAllAsync(cancellationToken);

        // ✅ Apply role-based filtering
        var filteredOrders = await ApplyRoleBasedOrderFilteringAsync(allOrders, cancellationToken);

        var today = DateTime.UtcNow;

        var delayedTasks = new List<DelayedTaskDto>();

        // Find delayed orders (only from accessible orders)
        var delayedOrders = filteredOrders
            .Where(o => o.DeadlineDate < today &&
                       o.Status != OrderStatus.Completed &&
                       o.Status != OrderStatus.Cancelled)
            .ToList();

        foreach (var order in delayedOrders)
        {
            var daysDelayed = (int)(today - order.DeadlineDate).TotalDays;

            delayedTasks.Add(new DelayedTaskDto
            {
                TaskId = null,
                TaskTitle = $"Order {order.OrderNumber}",
                OrderNumber = order.OrderNumber,
                TeamName = null,
                AssignedWorkerName = null,
                ExpectedCompletionDate = order.DeadlineDate,
                DaysDelayed = daysDelayed,
                Status = order.Status.ToString()
            });
        }

        // ✅ Filter tasks to only those related to accessible orders
        var accessibleOrderIds = filteredOrders.Select(o => o.Id).ToHashSet();

        // Find individual delayed tasks (tasks that started but not completed)
        var incompleteTasks = allTasks
            .Where(t => accessibleOrderIds.Contains(t.OrderId) &&  // ✅ Only tasks from accessible orders
                       t.StartedAt.HasValue &&
                       !t.CompletedAt.HasValue &&
                       t.Status != Domain.Enums.TaskStatus.Completed &&
                       t.Status != Domain.Enums.TaskStatus.Rejected)
            .ToList();

        foreach (var task in incompleteTasks)
        {
            var order = filteredOrders.FirstOrDefault(o => o.Id == task.OrderId);
            if (order == null) continue;

            // Estimate task deadline based on estimated hours (simple calculation)
            var estimatedCompletionDate = task.StartedAt.Value.AddHours((double)(task.EstimatedHours ?? 8));

            if (estimatedCompletionDate < today)
            {
                var daysDelayed = (int)(today - estimatedCompletionDate).TotalDays;

                delayedTasks.Add(new DelayedTaskDto
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    OrderNumber = order.OrderNumber,
                    TeamName = task.Team?.Name,
                    AssignedWorkerName = task.AssignedWorker != null
                        ? $"{task.AssignedWorker.FirstName} {task.AssignedWorker.LastName}"
                        : null,
                    ExpectedCompletionDate = estimatedCompletionDate,
                    DaysDelayed = daysDelayed,
                    Status = task.Status.ToString()
                });
            }
        }

        return delayedTasks.OrderByDescending(d => d.DaysDelayed);
    }

    public async Task<ProductionManagerDashboardDto> GetProductionManagerDashboardAsync(CancellationToken cancellationToken = default)
    {
        // ✅ Step 1: Get current ProductionManager's Employee ID
        var employeeId = await GetCurrentEmployeeIdAsync(cancellationToken);
        if (!employeeId.HasValue)
        {
            throw new UnauthorizedAccessException("ProductionManager employee record not found.");
        }

        var currentUserId = _currentUserService.UserId!.Value;

        // ✅ Step 2: Get ALL orders and filter to ProductionManager's assigned orders
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var assignedOrders = allOrders
            .Where(o => o.AssignedProductionManagerId == currentUserId)  // Filter by User.Id
            .ToList();

        // ✅ Step 3: Calculate order statistics by status
        var totalAssignedOrders = assignedOrders.Count;
        var ordersReadyForProduction = assignedOrders.Count(o => o.Status == OrderStatus.SpecificationsReady);
        var ordersInProduction = assignedOrders.Count(o => o.Status == OrderStatus.InProduction);
        var ordersInQualityCheck = assignedOrders.Count(o => o.Status == OrderStatus.QualityCheck);
        var ordersCompleted = assignedOrders.Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered);
        var delayedOrders = assignedOrders.Count(o => o.DeadlineDate < DateTime.UtcNow &&
                                                      o.Status != OrderStatus.Completed &&
                                                      o.Status != OrderStatus.Cancelled);

        // ✅ Step 4: Get ALL tasks and filter to ProductionManager's orders
        var allTasks = await _unitOfWork.WorkTasks.GetAllAsync(cancellationToken);
        var assignedOrderIds = assignedOrders.Select(o => o.Id).ToHashSet();
        var relevantTasks = allTasks.Where(t => assignedOrderIds.Contains(t.OrderId)).ToList();

        // ✅ Step 5: Calculate task statistics
        var totalTasks = relevantTasks.Count;
        var tasksInProgress = relevantTasks.Count(t => t.Status == Domain.Enums.TaskStatus.InProgress ||
                                                       t.Status == Domain.Enums.TaskStatus.Accepted);
        var tasksCompleted = relevantTasks.Count(t => t.Status == Domain.Enums.TaskStatus.Completed);
        var tasksPending = relevantTasks.Count(t => t.Status == Domain.Enums.TaskStatus.Pending);

        // ✅ Step 6: Calculate production metrics
        var averageProgress = ordersInProduction > 0
            ? assignedOrders.Where(o => o.Status == OrderStatus.InProduction).Average(o => o.ProgressPercentage)
            : 0;

        var completedTasks = relevantTasks.Where(t => t.Status == Domain.Enums.TaskStatus.Completed).ToList();
        var totalEstimatedHours = relevantTasks.Where(t => t.EstimatedHours.HasValue).Sum(t => t.EstimatedHours.Value);
        var totalActualHours = completedTasks.Where(t => t.ActualHours.HasValue).Sum(t => t.ActualHours.Value);
        var efficiencyPercentage = totalEstimatedHours > 0 && totalActualHours > 0
            ? Math.Round((totalEstimatedHours / totalActualHours) * 100, 2)
            : 0;

        // ✅ Step 7: Get delayed tasks (only from ProductionManager's orders)
        var delayedTasksList = await GetDelayedTasksForProductionManagerAsync(assignedOrders, relevantTasks, cancellationToken);

        // ✅ Step 8: Get constructors (employees with Constructor role working on ProductionManager's orders)
        var allUsers = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var constructorUserIds = allUsers
            .Where(u => u.Role == UserRole.Constructor && u.IsActive)
            .Select(u => u.Id)
            .ToHashSet();

        var constructorsOnOrders = assignedOrders
            .Where(o => o.AssignedConstructorId.HasValue && constructorUserIds.Contains(o.AssignedConstructorId.Value))
            .GroupBy(o => o.AssignedConstructorId!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                AssignedCount = g.Count(),
                CompletedCount = g.Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
            })
            .ToList();

        var constructorSummaries = new List<ConstructorSummaryDto>();
        foreach (var constructorData in constructorsOnOrders)
        {
            var user = allUsers.First(u => u.Id == constructorData.UserId);
            var employees = await _unitOfWork.Employees.FindAsync(e => e.UserId == user.Id && e.IsActive, cancellationToken);
            var employee = employees.FirstOrDefault();

            if (employee != null)
            {
                constructorSummaries.Add(new ConstructorSummaryDto
                {
                    EmployeeId = employee.Id,
                    FullName = employee.FullName,
                    AssignedOrdersCount = constructorData.AssignedCount,
                    CompletedOrdersCount = constructorData.CompletedCount
                });
            }
        }

        // ✅ Step 9: Count active workers and teams
        // TODO: Implement proper worker/team tracking based on your domain model
        var activeWorkers = 0;  // Placeholder
        var activeTeams = 0;     // Placeholder

        return new ProductionManagerDashboardDto
        {
            TotalAssignedOrders = totalAssignedOrders,
            OrdersReadyForProduction = ordersReadyForProduction,
            OrdersInProduction = ordersInProduction,
            OrdersInQualityCheck = ordersInQualityCheck,
            OrdersCompleted = ordersCompleted,
            DelayedOrders = delayedOrders,
            TotalTasks = totalTasks,
            TasksInProgress = tasksInProgress,
            TasksCompleted = tasksCompleted,
            TasksPending = tasksPending,
            AverageProgress = averageProgress,
            EfficiencyPercentage = efficiencyPercentage,
            ActiveWorkers = activeWorkers,
            ActiveTeams = activeTeams,
            DelayedTasks = delayedTasksList.ToList(),
            Constructors = constructorSummaries
        };
    }

    #region Private Helper Methods

    /// <summary>
    /// Resolves the current user's Employee ID from their User ID
    /// CRITICAL: Constructor/ProductionManager/Worker data is linked via Employees table
    /// This method provides centralized UserId → EmployeeId resolution for role-based filtering
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee ID if found, null otherwise</returns>
    private async Task<int?> GetCurrentEmployeeIdAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            return null;
        }

        var userId = _currentUserService.UserId.Value;

        var employees = await _unitOfWork.Employees
            .FindAsync(e => e.UserId == userId && e.IsActive, cancellationToken);

        var employee = employees.FirstOrDefault();
        return employee?.Id;
    }

    /// <summary>
    /// Gets delayed tasks for ProductionManager dashboard
    /// Only includes tasks from orders assigned to the ProductionManager
    /// </summary>
    private Task<IEnumerable<DelayedTaskDto>> GetDelayedTasksForProductionManagerAsync(
        List<FurniFlowUz.Domain.Entities.Order> assignedOrders,
        List<WorkTask> relevantTasks,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var delayedTasks = new List<DelayedTaskDto>();

        // Find delayed orders (only from assigned orders)
        var delayedOrders = assignedOrders
            .Where(o => o.DeadlineDate < today &&
                       o.Status != OrderStatus.Completed &&
                       o.Status != OrderStatus.Cancelled)
            .ToList();

        foreach (var order in delayedOrders)
        {
            var daysDelayed = (int)(today - order.DeadlineDate).TotalDays;

            delayedTasks.Add(new DelayedTaskDto
            {
                TaskId = null,
                TaskTitle = $"Order {order.OrderNumber}",
                OrderNumber = order.OrderNumber,
                TeamName = null,
                AssignedWorkerName = null,
                ExpectedCompletionDate = order.DeadlineDate,
                DaysDelayed = daysDelayed,
                Status = order.Status.ToString()
            });
        }

        // Find individual delayed tasks (tasks that started but not completed)
        var incompleteTasks = relevantTasks
            .Where(t => t.StartedAt.HasValue &&
                       !t.CompletedAt.HasValue &&
                       t.Status != Domain.Enums.TaskStatus.Completed &&
                       t.Status != Domain.Enums.TaskStatus.Rejected)
            .ToList();

        foreach (var task in incompleteTasks)
        {
            var order = assignedOrders.FirstOrDefault(o => o.Id == task.OrderId);
            if (order == null) continue;

            // Estimate task deadline based on estimated hours
            var estimatedCompletionDate = task.StartedAt.Value.AddHours((double)(task.EstimatedHours ?? 8));

            if (estimatedCompletionDate < today)
            {
                var daysDelayed = (int)(today - estimatedCompletionDate).TotalDays;

                delayedTasks.Add(new DelayedTaskDto
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    OrderNumber = order.OrderNumber,
                    TeamName = task.Team?.Name,
                    AssignedWorkerName = task.AssignedWorker != null
                        ? $"{task.AssignedWorker.FirstName} {task.AssignedWorker.LastName}"
                        : null,
                    ExpectedCompletionDate = estimatedCompletionDate,
                    DaysDelayed = daysDelayed,
                    Status = task.Status.ToString()
                });
            }
        }

        return Task.FromResult(delayedTasks.OrderByDescending(d => d.DaysDelayed).AsEnumerable());
    }

    /// <summary>
    /// Filters orders based on current user's role
    /// Constructor role: Returns only orders assigned to the constructor
    /// Other roles: Returns all orders (to be extended as needed)
    /// </summary>
    private async Task<IEnumerable<FurniFlowUz.Domain.Entities.Order>> ApplyRoleBasedOrderFilteringAsync(
        IEnumerable<FurniFlowUz.Domain.Entities.Order> orders,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            return Enumerable.Empty<FurniFlowUz.Domain.Entities.Order>();
        }

        var currentRole = _currentUserService.Role;

        if (!Enum.TryParse<UserRole>(currentRole, out var userRole))
        {
            return Enumerable.Empty<FurniFlowUz.Domain.Entities.Order>();
        }

        switch (userRole)
        {
            case UserRole.Director:
                // Directors see all orders
                return orders;

            case UserRole.Constructor:
                // Constructors see only orders assigned to them
                var currentUserId = _currentUserService.UserId.Value;
                return orders.Where(o => o.AssignedConstructorId == currentUserId);

            case UserRole.ProductionManager:
                // Production Managers see only orders assigned to them
                var pmUserId = _currentUserService.UserId.Value;
                return orders.Where(o => o.AssignedProductionManagerId == pmUserId);

            case UserRole.Salesperson:
                // Salespeople see only orders they created
                var salesUserId = _currentUserService.UserId.Value;
                return orders.Where(o => o.CreatedBy == salesUserId);

            default:
                return Enumerable.Empty<FurniFlowUz.Domain.Entities.Order>();
        }
    }

    #endregion
}
