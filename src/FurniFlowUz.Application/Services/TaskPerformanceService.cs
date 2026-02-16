using AutoMapper;
using FurniFlowUz.Application.DTOs.TaskPerformance;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for task performance and KPI tracking
/// </summary>
public class TaskPerformanceService : ITaskPerformanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public TaskPerformanceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<TaskPerformanceDto> GetByTaskIdAsync(int detailTaskId, CancellationToken cancellationToken = default)
    {
        var performance = (await _unitOfWork.TaskPerformances.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: p => p.DetailTaskId == detailTaskId,
            cancellationToken: cancellationToken)).FirstOrDefault();

        if (performance == null)
        {
            throw new NotFoundException($"Performance record not found for task {detailTaskId}");
        }

        return _mapper.Map<TaskPerformanceDto>(performance);
    }

    public async Task<EmployeeKPIDto> GetMyKPIAsync(DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        return await GetEmployeeKPIAsync(_currentUserService.UserId.Value, periodStart, periodEnd, cancellationToken);
    }

    public async Task<EmployeeKPIDto> GetEmployeeKPIAsync(int employeeId, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default)
    {
        // Set default period if not specified (last 30 days)
        var start = periodStart ?? DateTime.UtcNow.AddDays(-30);
        var end = periodEnd ?? DateTime.UtcNow;

        // Get employee details
        var employee = (await _unitOfWork.Employees.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: e => e.UserId == employeeId,
            includeProperties: "Position,Department",
            cancellationToken: cancellationToken)).FirstOrDefault();

        if (employee == null)
        {
            throw new NotFoundException($"Employee not found for user {employeeId}");
        }

        // Get all completed tasks by employee in period
        var completedTasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.AssignedEmployeeId == employeeId &&
                        (t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed) &&
                        t.CompletedAt.HasValue &&
                        t.CompletedAt.Value >= start &&
                        t.CompletedAt.Value <= end,
            includeProperties: "Performance",
            cancellationToken: cancellationToken);

        // Get performance records
        var performances = completedTasks
            .Where(t => t.Performance != null)
            .Select(t => t.Performance!)
            .ToList();

        // Get active tasks
        var activeTasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.AssignedEmployeeId == employeeId &&
                        t.Status == DetailTaskStatus.InProgress,
            cancellationToken: cancellationToken);

        // Get pending tasks
        var pendingTasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.AssignedEmployeeId == employeeId &&
                        (t.Status == DetailTaskStatus.Pending || t.Status == DetailTaskStatus.Ready),
            cancellationToken: cancellationToken);

        // Calculate KPIs
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var kpi = new EmployeeKPIDto
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.FullName,
            PositionName = employee.Position?.Name ?? "",
            DepartmentName = employee.Department?.Name ?? "",
            TotalTasksCompleted = completedTasks.Count(),
            TasksCompletedToday = completedTasks.Count(t => t.CompletedAt!.Value.Date == today),
            TasksCompletedThisWeek = completedTasks.Count(t => t.CompletedAt!.Value >= weekStart),
            TasksCompletedThisMonth = completedTasks.Count(t => t.CompletedAt!.Value >= monthStart),
            ActiveTasksCount = activeTasks.Count(),
            PendingTasksCount = pendingTasks.Count(),
            PeriodStart = start,
            PeriodEnd = end
        };

        if (performances.Any())
        {
            kpi.AverageEfficiency = Math.Round(performances.Average(p => p.EfficiencyPercent), 2);
            kpi.AverageQualityScore = Math.Round((decimal)performances.Average(p => p.QualityScore), 2);
            kpi.ReworkRate = Math.Round((decimal)performances.Count(p => p.RequiredRework) / performances.Count * 100, 2);

            // Calculate on-time completion (efficiency >= 100%)
            var onTimeTasks = performances.Count(p => p.EfficiencyPercent >= 100);
            kpi.OnTimeCompletionRate = Math.Round((decimal)onTimeTasks / performances.Count * 100, 2);
        }

        return kpi;
    }

    public async Task<TeamKPIDto> GetTeamKPIAsync(int teamId, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default)
    {
        var start = periodStart ?? DateTime.UtcNow.AddDays(-30);
        var end = periodEnd ?? DateTime.UtcNow;

        // Get team details
        var team = (await _unitOfWork.Teams.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: t => t.Id == teamId,
            includeProperties: "TeamLeader,Members",
            cancellationToken: cancellationToken)).FirstOrDefault();

        if (team == null)
        {
            throw new NotFoundException(nameof(Team), teamId);
        }

        // Get all team member IDs
        var teamMemberIds = team.Members?.Select(m => m.Id).ToList() ?? new List<int>();

        // Get all completed tasks by team members
        var completedTasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => t.AssignedEmployeeId.HasValue && teamMemberIds.Contains(t.AssignedEmployeeId.Value) &&
                        (t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed) &&
                        t.CompletedAt.HasValue &&
                        t.CompletedAt.Value >= start &&
                        t.CompletedAt.Value <= end,
            includeProperties: "Performance,AssignedEmployee",
            cancellationToken: cancellationToken);

        var performances = completedTasks
            .Where(t => t.Performance != null)
            .Select(t => t.Performance!)
            .ToList();

        // Get active assignments
        var activeAssignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.TeamId == teamId &&
                        (a.Status == CategoryAssignmentStatus.Assigned || a.Status == CategoryAssignmentStatus.InProgress),
            cancellationToken: cancellationToken);

        // Get completed assignments
        var completedAssignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.TeamId == teamId &&
                        a.Status == CategoryAssignmentStatus.Completed &&
                        a.CompletedAt.HasValue &&
                        a.CompletedAt.Value >= start &&
                        a.CompletedAt.Value <= end,
            cancellationToken: cancellationToken);

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var teamKPI = new TeamKPIDto
        {
            TeamId = team.Id,
            TeamName = team.Name,
            TeamLeaderName = $"{team.TeamLeader?.FirstName} {team.TeamLeader?.LastName}".Trim(),
            TotalMembers = team.Members?.Count ?? 0,
            ActiveMembers = team.Members?.Count(m => m.IsActive) ?? 0,
            TotalTasksCompleted = completedTasks.Count(),
            TasksCompletedToday = completedTasks.Count(t => t.CompletedAt!.Value.Date == today),
            TasksCompletedThisWeek = completedTasks.Count(t => t.CompletedAt!.Value >= weekStart),
            TasksCompletedThisMonth = completedTasks.Count(t => t.CompletedAt!.Value >= monthStart),
            ActiveAssignmentsCount = activeAssignments.Count(),
            CompletedAssignmentsCount = completedAssignments.Count(),
            PeriodStart = start,
            PeriodEnd = end
        };

        if (performances.Any())
        {
            teamKPI.AverageEfficiency = Math.Round(performances.Average(p => p.EfficiencyPercent), 2);
            teamKPI.AverageQualityScore = Math.Round((decimal)performances.Average(p => p.QualityScore), 2);
            teamKPI.ReworkRate = Math.Round((decimal)performances.Count(p => p.RequiredRework) / performances.Count * 100, 2);

            var onTimeTasks = performances.Count(p => p.EfficiencyPercent >= 100);
            teamKPI.OnTimeCompletionRate = Math.Round((decimal)onTimeTasks / performances.Count * 100, 2);
        }

        // Calculate top performers
        var employeePerformances = completedTasks
            .GroupBy(t => t.AssignedEmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Employee = g.First().AssignedEmployee,
                Tasks = g.ToList()
            })
            .OrderByDescending(e => e.Tasks.Count)
            .Take(5);

        foreach (var emp in employeePerformances)
        {
            var empPerformances = emp.Tasks.Where(t => t.Performance != null).Select(t => t.Performance!).ToList();
            if (empPerformances.Any())
            {
                teamKPI.TopPerformers.Add(new EmployeePerformanceSummary
                {
                    EmployeeId = emp.EmployeeId ?? 0,
                    EmployeeName = emp.Employee != null ? $"{emp.Employee.FirstName} {emp.Employee.LastName}".Trim() : "",
                    TasksCompleted = emp.Tasks.Count,
                    AverageEfficiency = Math.Round(empPerformances.Average(p => p.EfficiencyPercent), 2),
                    AverageQualityScore = Math.Round((decimal)empPerformances.Average(p => p.QualityScore), 2)
                });
            }
        }

        return teamKPI;
    }

    public async Task<TeamKPIDto> GetCompanyKPIAsync(DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default)
    {
        var start = periodStart ?? DateTime.UtcNow.AddDays(-30);
        var end = periodEnd ?? DateTime.UtcNow;

        // Get all completed tasks in period
        var completedTasks = await _unitOfWork.DetailTasks.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: t => (t.Status == DetailTaskStatus.Completed || t.Status == DetailTaskStatus.QCPassed) &&
                        t.CompletedAt.HasValue &&
                        t.CompletedAt.Value >= start &&
                        t.CompletedAt.Value <= end,
            includeProperties: "Performance,AssignedEmployee",
            cancellationToken: cancellationToken);

        var performances = completedTasks
            .Where(t => t.Performance != null)
            .Select(t => t.Performance!)
            .ToList();

        // Get all active assignments
        var activeAssignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.Status == CategoryAssignmentStatus.Assigned || a.Status == CategoryAssignmentStatus.InProgress,
            cancellationToken: cancellationToken);

        // Get all completed assignments in period
        var completedAssignments = await _unitOfWork.CategoryAssignments.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: a => a.Status == CategoryAssignmentStatus.Completed &&
                        a.CompletedAt.HasValue &&
                        a.CompletedAt.Value >= start &&
                        a.CompletedAt.Value <= end,
            cancellationToken: cancellationToken);

        // Get all active employees
        var activeEmployees = await _unitOfWork.Employees.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            filter: e => e.IsActive,
            cancellationToken: cancellationToken);

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var companyKPI = new TeamKPIDto
        {
            TeamId = 0,
            TeamName = "Company-Wide",
            TeamLeaderName = "All Teams",
            TotalMembers = activeEmployees.Count(),
            ActiveMembers = activeEmployees.Count(),
            TotalTasksCompleted = completedTasks.Count(),
            TasksCompletedToday = completedTasks.Count(t => t.CompletedAt!.Value.Date == today),
            TasksCompletedThisWeek = completedTasks.Count(t => t.CompletedAt!.Value >= weekStart),
            TasksCompletedThisMonth = completedTasks.Count(t => t.CompletedAt!.Value >= monthStart),
            ActiveAssignmentsCount = activeAssignments.Count(),
            CompletedAssignmentsCount = completedAssignments.Count(),
            PeriodStart = start,
            PeriodEnd = end
        };

        if (performances.Any())
        {
            companyKPI.AverageEfficiency = Math.Round(performances.Average(p => p.EfficiencyPercent), 2);
            companyKPI.AverageQualityScore = Math.Round((decimal)performances.Average(p => p.QualityScore), 2);
            companyKPI.ReworkRate = Math.Round((decimal)performances.Count(p => p.RequiredRework) / performances.Count * 100, 2);

            var onTimeTasks = performances.Count(p => p.EfficiencyPercent >= 100);
            companyKPI.OnTimeCompletionRate = Math.Round((decimal)onTimeTasks / performances.Count * 100, 2);
        }

        // Top performers across company
        var topEmployees = completedTasks
            .GroupBy(t => t.AssignedEmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Employee = g.First().AssignedEmployee,
                Tasks = g.ToList()
            })
            .OrderByDescending(e => e.Tasks.Count)
            .Take(10);

        foreach (var emp in topEmployees)
        {
            var empPerformances = emp.Tasks.Where(t => t.Performance != null).Select(t => t.Performance!).ToList();
            if (empPerformances.Any())
            {
                companyKPI.TopPerformers.Add(new EmployeePerformanceSummary
                {
                    EmployeeId = emp.EmployeeId ?? 0,
                    EmployeeName = emp.Employee != null ? $"{emp.Employee.FirstName} {emp.Employee.LastName}".Trim() : "",
                    TasksCompleted = emp.Tasks.Count,
                    AverageEfficiency = Math.Round(empPerformances.Average(p => p.EfficiencyPercent), 2),
                    AverageQualityScore = Math.Round((decimal)empPerformances.Average(p => p.QualityScore), 2)
                });
            }
        }

        return companyKPI;
    }

    public async Task<TaskPerformanceDto> RecordPerformanceAsync(CreateTaskPerformanceDto request, CancellationToken cancellationToken = default)
    {
        // Validate task exists and is completed
        var task = await _unitOfWork.DetailTasks.GetByIdAsync(request.DetailTaskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(DetailTask), request.DetailTaskId);
        }

        if (task.Status != DetailTaskStatus.Completed)
        {
            throw new ValidationException("Can only record performance for completed tasks.");
        }

        // Check if performance already recorded
        var existingPerformance = (await _unitOfWork.TaskPerformances.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: p => p.DetailTaskId == request.DetailTaskId,
            cancellationToken: cancellationToken)).FirstOrDefault();

        if (existingPerformance != null)
        {
            throw new ValidationException("Performance already recorded for this task.");
        }

        var performance = _mapper.Map<TaskPerformance>(request);

        // Calculate efficiency
        if (task.EstimatedDuration.HasValue)
        {
            performance.EfficiencyPercent = CalculateEfficiency(task.EstimatedDuration.Value, request.ActualDuration);
        }
        else
        {
            performance.EfficiencyPercent = 100; // Default if no estimate
        }

        await _unitOfWork.TaskPerformances.AddAsync(performance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskPerformanceDto>(performance);
    }

    public decimal CalculateEfficiency(TimeSpan estimatedDuration, TimeSpan actualDuration)
    {
        if (actualDuration.TotalMinutes == 0)
        {
            return 100;
        }

        var efficiency = (decimal)(estimatedDuration.TotalMinutes / actualDuration.TotalMinutes) * 100;
        return Math.Round(efficiency, 2);
    }
}
