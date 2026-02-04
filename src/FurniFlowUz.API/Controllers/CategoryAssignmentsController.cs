using FurniFlowUz.Application.DTOs.CategoryAssignment;
using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for category assignment management (Production Manager → Team Leader)
/// </summary>
[ApiController]
[Route("api/category-assignments")]
[Authorize(Roles = "ProductionManager,TeamLeader,Director")]
public class CategoryAssignmentsController : ControllerBase
{
    private readonly ICategoryAssignmentService _service;
    private readonly ILogger<CategoryAssignmentsController> _logger;

    public CategoryAssignmentsController(
        ICategoryAssignmentService service,
        ILogger<CategoryAssignmentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all category assignments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryAssignmentSummaryDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var assignments = await _service.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<CategoryAssignmentSummaryDto>>.SuccessResponse(
            assignments, "Category assignments retrieved successfully"));
    }

    /// <summary>
    /// Gets a category assignment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryAssignmentDto>>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var assignment = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<CategoryAssignmentDto>.SuccessResponse(
            assignment, "Category assignment retrieved successfully"));
    }

    /// <summary>
    /// Gets all assignments for a specific team leader
    /// </summary>
    [HttpGet("team-leader/{teamLeaderId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryAssignmentSummaryDto>>>> GetByTeamLeader(
        [FromRoute] int teamLeaderId,
        CancellationToken cancellationToken)
    {
        var assignments = await _service.GetByTeamLeaderAsync(teamLeaderId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<CategoryAssignmentSummaryDto>>.SuccessResponse(
            assignments, "Team leader assignments retrieved successfully"));
    }

    /// <summary>
    /// Gets all assignments for a specific order
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryAssignmentSummaryDto>>>> GetByOrder(
        [FromRoute] int orderId,
        CancellationToken cancellationToken)
    {
        var assignments = await _service.GetByOrderAsync(orderId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<CategoryAssignmentSummaryDto>>.SuccessResponse(
            assignments, "Order assignments retrieved successfully"));
    }

    /// <summary>
    /// Creates a new category assignment (Production Manager only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<CategoryAssignmentDto>>> Create(
        [FromBody] CreateCategoryAssignmentDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CategoryAssignmentDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var assignment = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<CategoryAssignmentDto>.SuccessResponse(
            assignment, "Category assigned successfully"));
    }

    /// <summary>
    /// Creates a simple category assignment by category name (Production Manager only)
    /// This is a simplified endpoint that auto-resolves FurnitureTypeId and TeamId
    /// </summary>
    [HttpPost("simple")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<CategoryAssignmentDto>>> CreateSimple(
        [FromBody] SimpleCategoryAssignmentDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CategoryAssignmentDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var assignment = await _service.CreateSimpleAsync(request, cancellationToken);
        return Ok(ApiResponse<CategoryAssignmentDto>.SuccessResponse(
            assignment, "Category assigned successfully"));
    }

    /// <summary>
    /// Updates category assignment status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<CategoryAssignmentDto>>> UpdateStatus(
        [FromRoute] int id,
        [FromBody] UpdateCategoryAssignmentDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CategoryAssignmentDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var assignment = await _service.UpdateStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<CategoryAssignmentDto>.SuccessResponse(
            assignment, "Assignment status updated successfully"));
    }

    /// <summary>
    /// Marks assignment as started
    /// </summary>
    [HttpPut("{id}/start")]
    [Authorize(Roles = "TeamLeader,Director")]
    public async Task<ActionResult<ApiResponse<CategoryAssignmentDto>>> StartAssignment(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var assignment = await _service.StartAssignmentAsync(id, cancellationToken);
        return Ok(ApiResponse<CategoryAssignmentDto>.SuccessResponse(
            assignment, "Assignment started successfully"));
    }

    /// <summary>
    /// Marks assignment as completed
    /// </summary>
    [HttpPut("{id}/complete")]
    [Authorize(Roles = "TeamLeader,Director")]
    public async Task<ActionResult<ApiResponse<CategoryAssignmentDto>>> CompleteAssignment(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var assignment = await _service.CompleteAssignmentAsync(id, cancellationToken);
        return Ok(ApiResponse<CategoryAssignmentDto>.SuccessResponse(
            assignment, "Assignment completed successfully"));
    }

    /// <summary>
    /// Deletes a category assignment (Production Manager only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Assignment deleted successfully"));
    }
}
