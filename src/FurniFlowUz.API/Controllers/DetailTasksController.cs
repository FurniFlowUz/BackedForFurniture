using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.DetailTask;
using FurniFlowUz.Application.DTOs.TaskPerformance;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for detail task management (Team Leader → Employee tasks)
/// </summary>
[ApiController]
[Route("api/detail-tasks")]
[Authorize]
public class DetailTasksController : ControllerBase
{
    private readonly IDetailTaskService _service;
    private readonly ILogger<DetailTasksController> _logger;

    public DetailTasksController(
        IDetailTaskService service,
        ILogger<DetailTasksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets a task by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DetailTaskDto>>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var task = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DetailTaskDto>.SuccessResponse(task, "Task retrieved successfully"));
    }

    /// <summary>
    /// Gets all tasks for a category assignment (Team Leader view)
    /// </summary>
    [HttpGet("category/{categoryAssignmentId}")]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DetailTaskListDto>>>> GetByCategoryAssignment(
        [FromRoute] int categoryAssignmentId,
        CancellationToken cancellationToken)
    {
        var tasks = await _service.GetByCategoryAssignmentAsync(categoryAssignmentId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<DetailTaskListDto>>.SuccessResponse(tasks, "Tasks retrieved successfully"));
    }

    /// <summary>
    /// Gets task queue for current employee
    /// </summary>
    [HttpGet("my-tasks")]
    [Authorize(Roles = "Employee,Worker,TeamLeader")]
    public async Task<ActionResult<ApiResponse<EmployeeTaskQueueDto>>> GetMyTaskQueue(
        CancellationToken cancellationToken)
    {
        var queue = await _service.GetMyTaskQueueAsync(cancellationToken);
        return Ok(ApiResponse<EmployeeTaskQueueDto>.SuccessResponse(queue, "Task queue retrieved successfully"));
    }

    /// <summary>
    /// Gets task queue for a specific employee
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<EmployeeTaskQueueDto>>> GetEmployeeTaskQueue(
        [FromRoute] int employeeId,
        CancellationToken cancellationToken)
    {
        var queue = await _service.GetEmployeeTaskQueueAsync(employeeId, cancellationToken);
        return Ok(ApiResponse<EmployeeTaskQueueDto>.SuccessResponse(queue, "Employee task queue retrieved successfully"));
    }

    /// <summary>
    /// Gets next available task for current employee
    /// </summary>
    [HttpGet("next")]
    [Authorize(Roles = "Employee,Worker,TeamLeader")]
    public async Task<ActionResult<ApiResponse<DetailTaskDto?>>> GetNextTask(
        CancellationToken cancellationToken)
    {
        var task = await _service.GetMyTaskQueueAsync(cancellationToken);
        return Ok(ApiResponse<DetailTaskDto?>.SuccessResponse(task.CurrentTask, "Next task retrieved successfully"));
    }

    /// <summary>
    /// Creates a new detail task (Team Leader only) - Full version
    /// </summary>
    [HttpPost("full")]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<DetailTaskDto>>> Create(
        [FromBody] CreateDetailTaskDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DetailTaskDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var task = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<DetailTaskDto>.SuccessResponse(task, "Task created successfully"));
    }

    /// <summary>
    /// Creates a new detail task (Team Leader only) - Simplified version
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<DetailTaskDto>>> CreateSimple(
        [FromBody] CreateSimpleDetailTaskDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DetailTaskDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var task = await _service.CreateSimpleAsync(request, cancellationToken);
        return Ok(ApiResponse<DetailTaskDto>.SuccessResponse(task, "Task created successfully"));
    }

    /// <summary>
    /// Updates task status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<DetailTaskDto>>> UpdateStatus(
        [FromRoute] int id,
        [FromBody] UpdateDetailTaskDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DetailTaskDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var task = await _service.UpdateStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DetailTaskDto>.SuccessResponse(task, "Task status updated successfully"));
    }

    /// <summary>
    /// Starts a task (Employee begins work)
    /// </summary>
    [HttpPut("{id}/start")]
    [Authorize(Roles = "Employee,Worker,TeamLeader")]
    public async Task<ActionResult<ApiResponse<DetailTaskDto>>> StartTask(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var task = await _service.StartTaskAsync(id, cancellationToken);
        return Ok(ApiResponse<DetailTaskDto>.SuccessResponse(task, "Task started successfully"));
    }

    /// <summary>
    /// Completes a task and records performance
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Employee,Worker,TeamLeader")]
    public async Task<ActionResult<ApiResponse<DetailTaskDto>>> CompleteTask(
        [FromRoute] int id,
        [FromBody] CreateTaskPerformanceDto performanceData,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DetailTaskDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var task = await _service.CompleteTaskAsync(id, performanceData, cancellationToken);
        return Ok(ApiResponse<DetailTaskDto>.SuccessResponse(task, "Task completed successfully"));
    }

    /// <summary>
    /// Deletes a task (Team Leader only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Task deleted successfully"));
    }
}
