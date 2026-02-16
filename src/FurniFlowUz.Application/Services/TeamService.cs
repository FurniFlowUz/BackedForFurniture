using AutoMapper;
using FurniFlowUz.Application.DTOs.Production;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Data;
using FurniFlowUz.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for team management
/// </summary>
public class TeamService : ITeamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;

    public TeamService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TeamDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Include TeamLeader and Members for proper mapping
        var teams = await _dbContext.Teams
            .Include(t => t.TeamLeader)
            .Include(t => t.Members)
            .Where(t => !t.IsDeleted)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }

    public async Task<TeamDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Include TeamLeader and Members for proper mapping
        var team = await _dbContext.Teams
            .Include(t => t.TeamLeader)
            .Include(t => t.Members)
            .Where(t => !t.IsDeleted && t.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

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

        // Allow Worker, Constructor roles to be added to teams
        if (worker.Role != UserRole.Worker && worker.Role != UserRole.Constructor)
        {
            throw new ValidationException("User must have Worker or Constructor role to be a team member.");
        }

        if (!worker.IsActive)
        {
            throw new ValidationException("Worker is not active.");
        }

        // Check if worker is already in the team using TeamMember table directly
        var isAlreadyMember = await _dbContext.Set<Dictionary<string, object>>("TeamMember")
            .AnyAsync(tm => EF.Property<int>(tm, "TeamId") == teamId && EF.Property<int>(tm, "UserId") == workerId, cancellationToken);

        if (isAlreadyMember)
        {
            throw new ValidationException("Worker is already a member of this team.");
        }

        // Add worker to team using raw SQL to insert into TeamMember table
        await _dbContext.Database.ExecuteSqlRawAsync(
            "INSERT INTO TeamMember (TeamId, UserId) VALUES ({0}, {1})",
            teamId, workerId);

        // Update team timestamp
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

        // Check if worker is in the team using TeamMember table directly
        var isMember = await _dbContext.Database.ExecuteSqlRawAsync(
            "SELECT 1 FROM TeamMember WHERE TeamId = {0} AND UserId = {1}",
            teamId, workerId) > 0;

        // Remove worker from team using raw SQL
        var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(
            "DELETE FROM TeamMember WHERE TeamId = {0} AND UserId = {1}",
            teamId, workerId);

        if (rowsAffected == 0)
        {
            throw new NotFoundException("Worker is not a member of this team.");
        }

        // Update team timestamp
        team.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Teams.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamDto>> GetTeamsByLeaderIdAsync(int leaderId, CancellationToken cancellationToken = default)
    {
        // Get teams where the specified user is the team leader
        var teams = await _dbContext.Teams
            .Include(t => t.TeamLeader)
            .Include(t => t.Members)
            .Where(t => !t.IsDeleted && t.TeamLeaderId == leaderId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<TeamDto>>(teams);
    }
}
