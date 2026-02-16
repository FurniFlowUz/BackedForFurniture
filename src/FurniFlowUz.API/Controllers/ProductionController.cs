using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.DTOs.Production;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for production management operations (tasks, teams, assignments)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ProductionManager,TeamLeader,Worker,Director")]
public class ProductionController : ControllerBase
{
    private readonly IProductionService _productionService;
    private readonly ITeamService _teamService;
    private readonly ILogger<ProductionController> _logger;

    public ProductionController(
        IProductionService productionService,
        ITeamService teamService,
        ILogger<ProductionController> logger)
    {
        _productionService = productionService;
        _teamService = teamService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all orders assigned to the current production manager
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of orders assigned to production manager</returns>
    [HttpGet("orders")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        var orders = await _productionService.GetOrdersByProductionManagerAsync(userId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
    }

    /// <summary>
    /// Gets all teams
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all teams</returns>
    [HttpGet("teams")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetTeams(
        CancellationToken cancellationToken)
    {
        var teams = await _teamService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<TeamDto>>.SuccessResponse(teams, "Teams retrieved successfully"));
    }

    /// <summary>
    /// Creates a new team
    /// </summary>
    /// <param name="request">Team creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created team information</returns>
    [HttpPost("teams")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<TeamDto>>> CreateTeam(
        [FromBody] CreateTeamDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<TeamDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var team = await _teamService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<TeamDto>.SuccessResponse(team, "Team created successfully"));
    }

    /// <summary>
    /// Updates an existing team
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="request">Updated team data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated team information</returns>
    [HttpPut("teams/{id}")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<TeamDto>>> UpdateTeam(
        [FromRoute] int id,
        [FromBody] UpdateTeamDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<TeamDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var team = await _teamService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<TeamDto>.SuccessResponse(team, "Team updated successfully"));
    }

    /// <summary>
    /// Deletes a team (soft delete)
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("teams/{id}")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTeam(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _teamService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Team deleted successfully"));
    }

    /// <summary>
    /// Gets all tasks (filtered by role - team leader sees their team's tasks, workers see their own)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of work tasks</returns>
    [HttpGet("tasks")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkTaskDto>>>> GetTasks(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var role = User.FindFirst("role")?.Value;

        IEnumerable<WorkTaskDto> tasks;

        if (role == "Worker")
        {
            tasks = await _productionService.GetTasksByWorkerAsync(userId, cancellationToken);
        }
        else if (role == "TeamLeader")
        {
            // Get the team ID for the team leader (assuming they lead one team)
            var teams = await _teamService.GetAllAsync(cancellationToken);
            var team = teams.FirstOrDefault(t => t.TeamLeader?.Id == userId);
            if (team != null && team.Id.HasValue)
            {
                tasks = await _productionService.GetTasksByTeamAsync(team.Id.Value, cancellationToken);
            }
            else
            {
                tasks = new List<WorkTaskDto>();
            }
        }
        else
        {
            // Production Manager or Director sees all tasks
            var teams = await _teamService.GetAllAsync(cancellationToken);
            var allTasks = new List<WorkTaskDto>();
            foreach (var team in teams)
            {
                if (team.Id.HasValue)
                {
                    var teamTasks = await _productionService.GetTasksByTeamAsync(team.Id.Value, cancellationToken);
                    allTasks.AddRange(teamTasks);
                }
            }
            tasks = allTasks;
        }

        return Ok(ApiResponse<IEnumerable<WorkTaskDto>>.SuccessResponse(tasks, "Tasks retrieved successfully"));
    }

    /// <summary>
    /// Creates a new work task
    /// </summary>
    /// <param name="request">Task creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created task information</returns>
    [HttpPost("tasks")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<WorkTaskDto>>> CreateTask(
        [FromBody] CreateWorkTaskDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<WorkTaskDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var task = await _productionService.CreateTaskAsync(request, cancellationToken);
        return Ok(ApiResponse<WorkTaskDto>.SuccessResponse(task, "Task created successfully"));
    }

    /// <summary>
    /// Assigns a task to a worker
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Assignment data containing worker ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("tasks/{id}/assign")]
    [Authorize(Roles = "TeamLeader,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> AssignTask(
        [FromRoute] int id,
        [FromBody] AssignTaskDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        await _productionService.AssignTaskAsync(id, request, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Task assigned successfully"));
    }

    /// <summary>
    /// Worker accepts a task assigned to them
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Accept task data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("tasks/{id}/accept")]
    [Authorize(Roles = "Worker,TeamLeader")]
    public async Task<ActionResult<ApiResponse<object>>> AcceptTask(
        [FromRoute] int id,
        [FromBody] AcceptTaskDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        await _productionService.AcceptTaskAsync(id, request, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Task accepted successfully"));
    }

    /// <summary>
    /// Marks a task as completed
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Complete task data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("tasks/{id}/complete")]
    [Authorize(Roles = "Worker,TeamLeader,ProductionManager")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteTask(
        [FromRoute] int id,
        [FromBody] CompleteTaskDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        await _productionService.CompleteTaskAsync(id, request, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Task completed successfully"));
    }

    /// <summary>
    /// Gets all production stages
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all production stages</returns>
    [HttpGet("stages")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductionStageDto>>>> GetStages(
        CancellationToken cancellationToken)
    {
        var stages = await _productionService.GetAllStagesAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<ProductionStageDto>>.SuccessResponse(stages, "Stages retrieved successfully"));
    }

    /// <summary>
    /// Gets tasks by assignment ID
    /// </summary>
    /// <param name="assignmentId">Assignment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of work tasks for the assignment</returns>
    [HttpGet("tasks/assignment/{assignmentId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkTaskDto>>>> GetTasksByAssignment(
        [FromRoute] int assignmentId,
        CancellationToken cancellationToken)
    {
        var tasks = await _productionService.GetTasksByAssignmentAsync(assignmentId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<WorkTaskDto>>.SuccessResponse(tasks, "Tasks retrieved successfully"));
    }

    /// <summary>
    /// Gets tasks by order ID
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of work tasks for the order</returns>
    [HttpGet("tasks/order/{orderId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkTaskDto>>>> GetTasksByOrder(
        [FromRoute] int orderId,
        CancellationToken cancellationToken)
    {
        var tasks = await _productionService.GetTasksByOrderAsync(orderId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<WorkTaskDto>>.SuccessResponse(tasks, "Tasks retrieved successfully"));
    }

    /// <summary>
    /// Starts a task (changes status from Pending to InProgress)
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("tasks/{id}/start")]
    [Authorize(Roles = "Worker,TeamLeader,ProductionManager")]
    public async Task<ActionResult<ApiResponse<object>>> StartTask(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _productionService.StartTaskAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Task started successfully"));
    }

    /// <summary>
    /// Creates tasks for all production stages for a given assignment
    /// </summary>
    /// <param name="request">Request containing assignment, order, and team IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("tasks/create-for-assignment")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> CreateTasksForAssignment(
        [FromBody] CreateTasksForAssignmentDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        await _productionService.CreateTasksForAssignmentAsync(
            request.AssignmentId,
            request.OrderId,
            request.TeamId,
            cancellationToken);

        return Ok(ApiResponse<object>.SuccessResponse(null, "Tasks created successfully"));
    }
}

/// <summary>
/// DTO for creating tasks for an assignment
/// </summary>
public class CreateTasksForAssignmentDto
{
    public int AssignmentId { get; set; }
    public int OrderId { get; set; }
    public int TeamId { get; set; }
}
