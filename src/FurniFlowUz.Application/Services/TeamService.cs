using AutoMapper;
using FurniFlowUz.Application.DTOs.Production;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for team management
/// </summary>
public class TeamService : ITeamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeamService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TeamDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var teams = await _unitOfWork.Teams.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }

    public async Task<TeamDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(id, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), id);
        }

        return _mapper.Map<TeamDto>(team);
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto request, CancellationToken cancellationToken = default)
    {
        // Validate team leader exists and has correct role
        var teamLeader = await _unitOfWork.Users.GetByIdAsync(request.TeamLeaderId, cancellationToken);
        if (teamLeader == null)
        {
            throw new NotFoundException(nameof(User), request.TeamLeaderId);
        }

        if (teamLeader.Role != UserRole.TeamLeader)
        {
            throw new ValidationException("User must have TeamLeader role.");
        }

        if (!teamLeader.IsActive)
        {
            throw new ValidationException("Team leader is not active.");
        }

        // Check if team name already exists
        var existingTeams = await _unitOfWork.Teams.FindAsync(
            t => t.Name.ToLower() == request.Name.ToLower(),
            cancellationToken);

        if (existingTeams.Any())
        {
            throw new ValidationException($"Team with name '{request.Name}' already exists.");
        }

        var team = new Team
        {
            Name = request.Name,
            TeamLeaderId = request.TeamLeaderId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Teams.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TeamDto>(team);
    }

    public async Task<TeamDto> UpdateAsync(int id, UpdateTeamDto request, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(id, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), id);
        }

        // Validate team leader if changed
        if (team.TeamLeaderId != request.TeamLeaderId)
        {
            var teamLeader = await _unitOfWork.Users.GetByIdAsync(request.TeamLeaderId, cancellationToken);
            if (teamLeader == null)
            {
                throw new NotFoundException(nameof(User), request.TeamLeaderId);
            }

            if (teamLeader.Role != UserRole.TeamLeader)
            {
                throw new ValidationException("User must have TeamLeader role.");
            }

            if (!teamLeader.IsActive)
            {
                throw new ValidationException("Team leader is not active.");
            }
        }

        // Check if new name conflicts with existing teams
        if (team.Name != request.Name)
        {
            var existingTeams = await _unitOfWork.Teams.FindAsync(
                t => t.Name.ToLower() == request.Name.ToLower() && t.Id != id,
                cancellationToken);

            if (existingTeams.Any())
            {
                throw new ValidationException($"Team with name '{request.Name}' already exists.");
            }
        }

        team.Name = request.Name;
        team.TeamLeaderId = request.TeamLeaderId;
        team.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Teams.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TeamDto>(team);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(id, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), id);
        }

        // Soft delete by setting IsActive to false
        team.IsActive = false;
        team.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Teams.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMemberAsync(int teamId, int workerId, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), teamId);
        }

        // Validate worker exists and has correct role
        var worker = await _unitOfWork.Users.GetByIdAsync(workerId, cancellationToken);
        if (worker == null)
        {
            throw new NotFoundException(nameof(User), workerId);
        }

        if (worker.Role != UserRole.Worker)
        {
            throw new ValidationException("User must have Worker role.");
        }

        if (!worker.IsActive)
        {
            throw new ValidationException("Worker is not active.");
        }

        // Check if worker is already in the team
        if (team.Members.Any(m => m.Id == workerId))
        {
            throw new ValidationException("Worker is already a member of this team.");
        }

        // Add worker to team
        team.Members.Add(worker);
        team.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Teams.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveMemberAsync(int teamId, int workerId, CancellationToken cancellationToken = default)
    {
        var team = await _unitOfWork.Teams.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), teamId);
        }

        // Find worker in team
        var worker = team.Members.FirstOrDefault(m => m.Id == workerId);
        if (worker == null)
        {
            throw new NotFoundException("Worker is not a member of this team.");
        }

        // Remove worker from team
        team.Members.Remove(worker);
        team.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Teams.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
