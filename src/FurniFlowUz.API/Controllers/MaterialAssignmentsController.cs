using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.MaterialAssignment;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for material assignment management (Warehouse → Team/Employee)
/// </summary>
[ApiController]
[Route("api/material-assignments")]
[Authorize(Roles = "WarehouseManager,TeamLeader,Employee,Director")]
public class MaterialAssignmentsController : ControllerBase
{
    private readonly IMaterialAssignmentService _service;
    private readonly ILogger<MaterialAssignmentsController> _logger;

    public MaterialAssignmentsController(
        IMaterialAssignmentService service,
        ILogger<MaterialAssignmentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all material assignments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaterialAssignmentDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var assignments = await _service.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<MaterialAssignmentDto>>.SuccessResponse(
            assignments, "Material assignments retrieved successfully"));
    }

    /// <summary>
    /// Gets a material assignment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MaterialAssignmentDto>>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var assignment = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<MaterialAssignmentDto>.SuccessResponse(
            assignment, "Material assignment retrieved successfully"));
    }

    /// <summary>
    /// Gets all assignments for a specific team
    /// </summary>
    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaterialAssignmentDto>>>> GetByTeam(
        [FromRoute] int teamId,
        CancellationToken cancellationToken)
    {
        var assignments = await _service.GetByTeamAsync(teamId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<MaterialAssignmentDto>>.SuccessResponse(
            assignments, "Team material assignments retrieved successfully"));
    }

    /// <summary>
    /// Gets all assignments for a specific employee
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaterialAssignmentDto>>>> GetByEmployee(
        [FromRoute] int employeeId,
        CancellationToken cancellationToken)
    {
        var assignments = await _service.GetByEmployeeAsync(employeeId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<MaterialAssignmentDto>>.SuccessResponse(
            assignments, "Employee material assignments retrieved successfully"));
    }

    /// <summary>
    /// Gets pending (unconfirmed) assignments
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaterialAssignmentDto>>>> GetPending(
        CancellationToken cancellationToken)
    {
        var assignments = await _service.GetPendingAssignmentsAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<MaterialAssignmentDto>>.SuccessResponse(
            assignments, "Pending assignments retrieved successfully"));
    }

    /// <summary>
    /// Assigns material to a team (Warehouse Manager only)
    /// </summary>
    [HttpPost("assign-to-team")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<MaterialAssignmentDto>>> AssignToTeam(
        [FromBody] CreateMaterialAssignmentDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<MaterialAssignmentDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var assignment = await _service.AssignToTeamAsync(request, cancellationToken);
        return Ok(ApiResponse<MaterialAssignmentDto>.SuccessResponse(
            assignment, "Material assigned to team successfully"));
    }

    /// <summary>
    /// Assigns material to an employee (Warehouse Manager only)
    /// </summary>
    [HttpPost("assign-to-employee")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<MaterialAssignmentDto>>> AssignToEmployee(
        [FromBody] CreateMaterialAssignmentDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<MaterialAssignmentDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var assignment = await _service.AssignToEmployeeAsync(request, cancellationToken);
        return Ok(ApiResponse<MaterialAssignmentDto>.SuccessResponse(
            assignment, "Material assigned to employee successfully"));
    }

    /// <summary>
    /// Confirms receipt of assigned materials
    /// </summary>
    [HttpPut("{id}/confirm-receipt")]
    [Authorize(Roles = "TeamLeader,Employee,Director")]
    public async Task<ActionResult<ApiResponse<MaterialAssignmentDto>>> ConfirmReceipt(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var assignment = await _service.ConfirmReceiptAsync(id, cancellationToken);
        return Ok(ApiResponse<MaterialAssignmentDto>.SuccessResponse(
            assignment, "Receipt confirmed successfully"));
    }

    /// <summary>
    /// Deletes a material assignment (Warehouse Manager only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Material assignment deleted successfully"));
    }
}
