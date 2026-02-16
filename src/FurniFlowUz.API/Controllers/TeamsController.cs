using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Production;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for team management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ProductionManager,TeamLeader,Director")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;
    private readonly ILogger<TeamsController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public TeamsController(ITeamService teamService, ILogger<TeamsController> logger, ICurrentUserService currentUserService)
    {
        _teamService = teamService;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all teams
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all teams</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetTeams(
        CancellationToken cancellationToken)
    {
        var teams = await _teamService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<TeamDto>>.SuccessResponse(teams, "Teams retrieved successfully"));
    }

    /// <summary>
    /// Gets teams where current user is the team leader
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of teams led by current user</returns>
    [HttpGet("my-teams")]
    [Authorize(Roles = "TeamLeader")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetMyTeams(
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return Unauthorized(ApiResponse<IEnumerable<TeamDto>>.FailureResponse("User not authenticated"));
        }

        var teams = await _teamService.GetTeamsByLeaderIdAsync(currentUserId.Value, cancellationToken);
        return Ok(ApiResponse<IEnumerable<TeamDto>>.SuccessResponse(teams, "My teams retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific team by ID with all members
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed team information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TeamDto>>> GetTeam(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var team = await _teamService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<TeamDto>.SuccessResponse(team, "Team retrieved successfully"));
    }

    /// <summary>
    /// Creates a new team
    /// </summary>
    /// <param name="request">Team creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created team information</returns>
    [HttpPost]
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
        return CreatedAtAction(nameof(GetTeam), new { id = team.Id },
            ApiResponse<TeamDto>.SuccessResponse(team, "Team created successfully"));
    }

    /// <summary>
    /// Updates an existing team
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="request">Updated team data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated team information</returns>
    [HttpPut("{id}")]
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
    [HttpDelete("{id}")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTeam(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _teamService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Team deleted successfully"));
    }

    /// <summary>
    /// Adds a member (worker) to a team
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="userId">User ID of the worker to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("{id}/members")]
    [Authorize(Roles = "ProductionManager,TeamLeader,Director")]
    public async Task<ActionResult<ApiResponse<object>>> AddMember(
        [FromRoute] int id,
        [FromBody] int userId,
        CancellationToken cancellationToken)
    {
        await _teamService.AddMemberAsync(id, userId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Member added to team successfully"));
    }

    /// <summary>
    /// Removes a member (worker) from a team
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="userId">User ID of the worker to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}/members/{userId}")]
    [Authorize(Roles = "ProductionManager,TeamLeader,Director")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveMember(
        [FromRoute] int id,
        [FromRoute] int userId,
        CancellationToken cancellationToken)
    {
        await _teamService.RemoveMemberAsync(id, userId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Member removed from team successfully"));
    }
}
