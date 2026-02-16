using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.TaskPerformance;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for employee KPI and performance tracking
/// </summary>
[ApiController]
[Route("api/kpi")]
[Authorize]
public class EmployeeKPIController : ControllerBase
{
    private readonly ITaskPerformanceService _service;
    private readonly ILogger<EmployeeKPIController> _logger;

    public EmployeeKPIController(
        ITaskPerformanceService service,
        ILogger<EmployeeKPIController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets KPI metrics for current employee
    /// </summary>
    [HttpGet("my-kpi")]
    [Authorize(Roles = "Employee,Worker,TeamLeader")]
    public async Task<ActionResult<ApiResponse<EmployeeKPIDto>>> GetMyKPI(
        [FromQuery] DateTime? periodStart,
        [FromQuery] DateTime? periodEnd,
        CancellationToken cancellationToken)
    {
        var kpi = await _service.GetMyKPIAsync(periodStart, periodEnd, cancellationToken);
        return Ok(ApiResponse<EmployeeKPIDto>.SuccessResponse(kpi, "Employee KPI retrieved successfully"));
    }

    /// <summary>
    /// Gets KPI metrics for a specific employee
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<EmployeeKPIDto>>> GetEmployeeKPI(
        [FromRoute] int employeeId,
        [FromQuery] DateTime? periodStart,
        [FromQuery] DateTime? periodEnd,
        CancellationToken cancellationToken)
    {
        var kpi = await _service.GetEmployeeKPIAsync(employeeId, periodStart, periodEnd, cancellationToken);
        return Ok(ApiResponse<EmployeeKPIDto>.SuccessResponse(kpi, "Employee KPI retrieved successfully"));
    }

    /// <summary>
    /// Gets KPI metrics for a team
    /// </summary>
    [HttpGet("team/{teamId}")]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<TeamKPIDto>>> GetTeamKPI(
        [FromRoute] int teamId,
        [FromQuery] DateTime? periodStart,
        [FromQuery] DateTime? periodEnd,
        CancellationToken cancellationToken)
    {
        var kpi = await _service.GetTeamKPIAsync(teamId, periodStart, periodEnd, cancellationToken);
        return Ok(ApiResponse<TeamKPIDto>.SuccessResponse(kpi, "Team KPI retrieved successfully"));
    }

    /// <summary>
    /// Gets company-wide KPI metrics (Director only)
    /// </summary>
    [HttpGet("company")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ApiResponse<TeamKPIDto>>> GetCompanyKPI(
        [FromQuery] DateTime? periodStart,
        [FromQuery] DateTime? periodEnd,
        CancellationToken cancellationToken)
    {
        var kpi = await _service.GetCompanyKPIAsync(periodStart, periodEnd, cancellationToken);
        return Ok(ApiResponse<TeamKPIDto>.SuccessResponse(kpi, "Company KPI retrieved successfully"));
    }

    /// <summary>
    /// Gets performance record for a specific task
    /// </summary>
    [HttpGet("task/{detailTaskId}/performance")]
    public async Task<ActionResult<ApiResponse<TaskPerformanceDto>>> GetTaskPerformance(
        [FromRoute] int detailTaskId,
        CancellationToken cancellationToken)
    {
        var performance = await _service.GetByTaskIdAsync(detailTaskId, cancellationToken);
        return Ok(ApiResponse<TaskPerformanceDto>.SuccessResponse(performance, "Task performance retrieved successfully"));
    }
}
