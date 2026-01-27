using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Position;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for position management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Director,ProductionManager")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService _positionService;
    private readonly ILogger<PositionsController> _logger;

    public PositionsController(IPositionService positionService, ILogger<PositionsController> logger)
    {
        _positionService = positionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all positions
    /// </summary>
    /// <returns>List of all positions</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PositionDto>>>> GetPositions()
    {
        var positions = await _positionService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<PositionDto>>.SuccessResponse(positions, "Positions retrieved successfully"));
    }

    /// <summary>
    /// Gets position by ID
    /// </summary>
    /// <param name="id">Position ID</param>
    /// <returns>Position information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PositionDto>>> GetPosition(int id)
    {
        var position = await _positionService.GetByIdAsync(id);
        return Ok(ApiResponse<PositionDto>.SuccessResponse(position, "Position retrieved successfully"));
    }

    /// <summary>
    /// Creates a new position
    /// </summary>
    /// <param name="request">Position creation data</param>
    /// <returns>Created position information</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PositionDto>>> CreatePosition(
        [FromBody] CreatePositionDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<PositionDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var position = await _positionService.CreateAsync(request);
        return Ok(ApiResponse<PositionDto>.SuccessResponse(position, "Position created successfully"));
    }

    /// <summary>
    /// Updates an existing position
    /// </summary>
    /// <param name="id">Position ID</param>
    /// <param name="request">Position update data</param>
    /// <returns>Updated position information</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<PositionDto>>> UpdatePosition(
        int id,
        [FromBody] UpdatePositionDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<PositionDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var position = await _positionService.UpdateAsync(id, request);
        return Ok(ApiResponse<PositionDto>.SuccessResponse(position, "Position updated successfully"));
    }

    /// <summary>
    /// Deletes a position
    /// </summary>
    /// <param name="id">Position ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePosition(int id)
    {
        var result = await _positionService.DeleteAsync(id);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Position deleted successfully"));
    }
}
