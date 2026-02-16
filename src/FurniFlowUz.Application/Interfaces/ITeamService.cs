using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for team management
/// </summary>
public interface ITeamService
{
    /// <summary>
    /// Gets all teams
    /// </summary>
    Task<IEnumerable<TeamDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a team by ID with all members
    /// </summary>
    Task<TeamDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new team
    /// </summary>
    Task<TeamDto> CreateAsync(CreateTeamDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing team
    /// </summary>
    Task<TeamDto> UpdateAsync(int id, UpdateTeamDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a team (soft delete by setting IsActive to false)
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a worker to a team
    /// </summary>
    Task AddMemberAsync(int teamId, int workerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a worker from a team
    /// </summary>
    Task RemoveMemberAsync(int teamId, int workerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams where the specified user is the team leader
    /// </summary>
    Task<IEnumerable<TeamDto>> GetTeamsByLeaderIdAsync(int leaderId, CancellationToken cancellationToken = default);
}
