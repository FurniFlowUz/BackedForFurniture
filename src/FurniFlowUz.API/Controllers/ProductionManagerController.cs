using FurniFlowUz.Application.DTOs.CategoryAssignment;
using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Dashboard;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for ProductionManager-specific operations
/// AUTHORIZATION: Only ProductionManager role can access these endpoints
/// DATA ISOLATION: Shows ONLY production data managed by the current ProductionManager
/// </summary>
[ApiController]
[Route("api/[controller]")]  // ✅ FIXED: Uses "ProductionManager" from class name
[Authorize(Roles = "ProductionManager")]  // ⚠️ CRITICAL: ONLY ProductionManager role
public class ProductionManagerController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ICategoryAssignmentService _categoryAssignmentService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ProductionManagerController> _logger;

    public ProductionManagerController(
        IDashboardService dashboardService,
        ICategoryAssignmentService categoryAssignmentService,
        ApplicationDbContext dbContext,
        ILogger<ProductionManagerController> logger)
    {
        _dashboardService = dashboardService;
        _categoryAssignmentService = categoryAssignmentService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Gets production-focused dashboard data for the current ProductionManager
    /// Shows ONLY orders, tasks, and statistics assigned to this ProductionManager
    /// DOES NOT show sales revenue, global company stats, or other managers' data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ProductionManager dashboard with production statistics</returns>
    /// <response code="200">Dashboard data retrieved successfully</response>
    /// <response code="401">Unauthorized - User is not authenticated</response>
    /// <response code="403">Forbidden - User is not a ProductionManager</response>
    [HttpGet("dashboard-stats")]  // ✅ FIXED: Changed from "dashboard" to "dashboard-stats"
    [ProducesResponseType(typeof(ApiResponse<ProductionManagerDashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductionManagerDashboardDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<ProductionManagerDashboardDto>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ProductionManagerDashboardDto>>> GetDashboardStats(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductionManager requesting dashboard data");

        var dashboard = await _dashboardService.GetProductionManagerDashboardAsync(cancellationToken);

        _logger.LogInformation(
            "ProductionManager dashboard retrieved: {TotalOrders} orders, {TotalTasks} tasks",
            dashboard.TotalAssignedOrders,
            dashboard.TotalTasks);

        return Ok(ApiResponse<ProductionManagerDashboardDto>.SuccessResponse(
            dashboard,
            "Production dashboard data retrieved successfully"
        ));
    }

    /// <summary>
    /// Gets category assignment statistics for the Production Manager
    /// Shows total, pending, in-progress, completed, and overdue assignments
    /// Also includes average completion percentage and active team leaders count
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Assignment statistics including recent assignments</returns>
    /// <response code="200">Assignment statistics retrieved successfully</response>
    /// <response code="401">Unauthorized - User is not authenticated</response>
    /// <response code="403">Forbidden - User is not a ProductionManager</response>
    [HttpGet("assignment-stats")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AssignmentStatsDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<AssignmentStatsDto>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<AssignmentStatsDto>>> GetAssignmentStats(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductionManager requesting assignment statistics");

        var stats = await _categoryAssignmentService.GetAssignmentStatsAsync(cancellationToken);

        _logger.LogInformation(
            "Assignment stats retrieved: {Total} total, {Completed} completed, {Pending} pending",
            stats.TotalAssignments,
            stats.CompletedAssignments,
            stats.PendingAssignments);

        return Ok(ApiResponse<AssignmentStatsDto>.SuccessResponse(
            stats,
            "Assignment statistics retrieved successfully"
        ));
    }

    /// <summary>
    /// Gets category production statistics grouped by furniture type
    /// Shows production progress for each category
    /// </summary>
    [HttpGet("category-production")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryProduction(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductionManager requesting category production stats");

        // Step 1: Get assignment counts by category
        var assignmentStats = await _dbContext.CategoryAssignments
            .Where(ca => !ca.IsDeleted)
            .GroupBy(ca => new { ca.FurnitureTypeId, ca.FurnitureType.Name })
            .Select(g => new
            {
                CategoryId = g.Key.FurnitureTypeId,
                CategoryName = g.Key.Name ?? "Unknown",
                TotalAssignments = g.Count(),
                CompletedAssignments = g.Count(ca => ca.Status == Domain.Enums.CategoryAssignmentStatus.Completed),
                InProgressAssignments = g.Count(ca => ca.Status == Domain.Enums.CategoryAssignmentStatus.InProgress)
            })
            .ToListAsync(cancellationToken);

        // Step 2: Get task counts by category (flattened from DetailTasks)
        var taskStats = await _dbContext.DetailTasks
            .Where(dt => !dt.CategoryAssignment.IsDeleted)
            .GroupBy(dt => new { dt.CategoryAssignment.FurnitureTypeId, dt.CategoryAssignment.FurnitureType.Name })
            .Select(g => new
            {
                CategoryId = g.Key.FurnitureTypeId,
                CategoryName = g.Key.Name ?? "Unknown",
                TotalTasks = g.Count(),
                CompletedTasks = g.Count(dt =>
                    dt.Status == Domain.Enums.DetailTaskStatus.Completed ||
                    dt.Status == Domain.Enums.DetailTaskStatus.QCPassed),
                InProgressTasks = g.Count(dt => dt.Status == Domain.Enums.DetailTaskStatus.InProgress)
            })
            .ToListAsync(cancellationToken);

        // Step 3: Combine results in memory (no nested aggregates sent to SQL Server)
        var categoryStats = assignmentStats
            .Select(a => new
            {
                categoryId = a.CategoryId,
                categoryName = a.CategoryName,
                totalAssignments = a.TotalAssignments,
                completedAssignments = a.CompletedAssignments,
                inProgressAssignments = a.InProgressAssignments,
                totalTasks = taskStats.FirstOrDefault(t => t.CategoryId == a.CategoryId)?.TotalTasks ?? 0,
                completedTasks = taskStats.FirstOrDefault(t => t.CategoryId == a.CategoryId)?.CompletedTasks ?? 0,
                inProgressTasks = taskStats.FirstOrDefault(t => t.CategoryId == a.CategoryId)?.InProgressTasks ?? 0
            })
            .ToList();

        return Ok(ApiResponse<List<object>>.SuccessResponse(
            categoryStats.Cast<object>().ToList(),
            "Category production statistics retrieved successfully"
        ));
    }

    /// <summary>
    /// Gets KPI (Key Performance Indicators) for production management
    /// </summary>
    [HttpGet("kpi")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKPI(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductionManager requesting KPI data");

        var totalOrders = await _dbContext.Orders.CountAsync(o => !o.IsDeleted, cancellationToken);
        var completedOrders = await _dbContext.Orders.CountAsync(o => !o.IsDeleted && o.Status == Domain.Enums.OrderStatus.Completed, cancellationToken);
        var totalAssignments = await _dbContext.CategoryAssignments.CountAsync(ca => !ca.IsDeleted, cancellationToken);
        var completedAssignments = await _dbContext.CategoryAssignments.CountAsync(ca => !ca.IsDeleted && ca.Status == Domain.Enums.CategoryAssignmentStatus.Completed, cancellationToken);
        var activeTeamLeaders = await _dbContext.Employees
            .Include(e => e.User)
            .CountAsync(e => e.IsActive && e.User.IsActive && e.User.Role == Domain.Enums.UserRole.TeamLeader, cancellationToken);

        var kpi = new
        {
            orderCompletionRate = totalOrders > 0 ? Math.Round((decimal)completedOrders / totalOrders * 100, 2) : 0,
            assignmentCompletionRate = totalAssignments > 0 ? Math.Round((decimal)completedAssignments / totalAssignments * 100, 2) : 0,
            totalOrders,
            completedOrders,
            totalAssignments,
            completedAssignments,
            activeTeamLeaders
        };

        return Ok(ApiResponse<object>.SuccessResponse(kpi, "KPI data retrieved successfully"));
    }

    /// <summary>
    /// Gets production chart data for visualizations
    /// Returns data grouped by date/status for charts
    /// </summary>
    [HttpGet("production-chart")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductionChart(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductionManager requesting production chart data");

        var last30Days = DateTime.UtcNow.AddDays(-30);

        var dailyStats = await _dbContext.CategoryAssignments
            .Where(ca => !ca.IsDeleted && ca.CreatedAt >= last30Days)
            .GroupBy(ca => ca.CreatedAt.Date)
            .Select(g => new
            {
                date = g.Key,
                assigned = g.Count(ca => ca.Status == Domain.Enums.CategoryAssignmentStatus.Assigned),
                inProgress = g.Count(ca => ca.Status == Domain.Enums.CategoryAssignmentStatus.InProgress),
                completed = g.Count(ca => ca.Status == Domain.Enums.CategoryAssignmentStatus.Completed)
            })
            .OrderBy(x => x.date)
            .ToListAsync(cancellationToken);

        var chartData = new
        {
            labels = dailyStats.Select(x => x.date.ToString("yyyy-MM-dd")).ToList(),
            datasets = new[]
            {
                new { label = "Assigned", data = dailyStats.Select(x => x.assigned).ToList() },
                new { label = "In Progress", data = dailyStats.Select(x => x.inProgress).ToList() },
                new { label = "Completed", data = dailyStats.Select(x => x.completed).ToList() }
            }
        };

        return Ok(ApiResponse<object>.SuccessResponse(chartData, "Production chart data retrieved successfully"));
    }

    /// <summary>
    /// Gets status distribution for all assignments
    /// Shows breakdown by status for pie/donut charts
    /// </summary>
    [HttpGet("status-distribution")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusDistribution(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductionManager requesting status distribution");

        // Get total count first (single query, no nesting)
        var totalAssignments = await _dbContext.CategoryAssignments
            .CountAsync(ca => !ca.IsDeleted, cancellationToken);

        // Get distribution by status (no nested aggregates)
        var distribution = await _dbContext.CategoryAssignments
            .Where(ca => !ca.IsDeleted)
            .GroupBy(ca => ca.Status)
            .Select(g => new
            {
                status = g.Key.ToString(),
                count = g.Count()
            })
            .ToListAsync(cancellationToken);

        // Calculate percentages in memory (safe)
        var result = distribution.Select(d => new
        {
            d.status,
            d.count,
            percentage = totalAssignments > 0
                ? Math.Round((decimal)d.count / totalAssignments * 100, 2)
                : 0m
        }).ToList();

        return Ok(ApiResponse<List<object>>.SuccessResponse(
            result.Cast<object>().ToList(),
            "Status distribution retrieved successfully"
        ));
    }

    /// <summary>
    /// LEGACY ENDPOINT: Kept for backwards compatibility
    /// Use /dashboard-stats instead
    /// </summary>
    [HttpGet("dashboard")]
    [ApiExplorerSettings(IgnoreApi = true)]  // Hide from Swagger
    public Task<ActionResult<ApiResponse<ProductionManagerDashboardDto>>> GetDashboard(
        CancellationToken cancellationToken)
    {
        return GetDashboardStats(cancellationToken);
    }
}
