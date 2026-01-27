using AutoMapper;
using FurniFlowUz.Application.DTOs.Position;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

public class PositionService : IPositionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PositionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PositionDto>> GetAllAsync()
    {
        var positions = await _unitOfWork.Positions.GetAllAsync();
        return _mapper.Map<IEnumerable<PositionDto>>(positions);
    }

    public async Task<PositionDto?> GetByIdAsync(int id)
    {
        var position = await _unitOfWork.Positions.GetByIdAsync(id);
        if (position == null)
        {
            throw new NotFoundException(nameof(Position), id);
        }

        return _mapper.Map<PositionDto>(position);
    }

    public async Task<PositionDto> CreateAsync(CreatePositionDto dto)
    {
        // Check if position name already exists
        var existingPositions = await _unitOfWork.Positions.FindAsync(
            p => p.Name.ToLower() == dto.Name.ToLower());

        if (existingPositions.Any())
        {
            throw new ValidationException($"Position with name '{dto.Name}' already exists.");
        }

        var position = _mapper.Map<Position>(dto);
        await _unitOfWork.Positions.AddAsync(position);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PositionDto>(position);
    }

    public async Task<PositionDto> UpdateAsync(int id, UpdatePositionDto dto)
    {
        var position = await _unitOfWork.Positions.GetByIdAsync(id);
        if (position == null)
        {
            throw new NotFoundException(nameof(Position), id);
        }

        // Check if new name already exists (excluding current position)
        var existingPositions = await _unitOfWork.Positions.FindAsync(
            p => p.Name.ToLower() == dto.Name.ToLower() && p.Id != id);

        if (existingPositions.Any())
        {
            throw new ValidationException($"Position with name '{dto.Name}' already exists.");
        }

        position.Name = dto.Name;
        position.Description = dto.Description;

        _unitOfWork.Positions.Update(position);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PositionDto>(position);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var position = await _unitOfWork.Positions.GetByIdAsync(id);
        if (position == null)
        {
            throw new NotFoundException(nameof(Position), id);
        }

        // Check if position is being used by any employees
        var employees = await _unitOfWork.Employees.FindAsync(e => e.PositionId == id);
        if (employees.Any())
        {
            throw new ValidationException("Cannot delete position that is assigned to employees.");
        }

        _unitOfWork.Positions.Remove(position);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
