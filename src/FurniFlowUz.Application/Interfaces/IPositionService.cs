using FurniFlowUz.Application.DTOs.Position;

namespace FurniFlowUz.Application.Interfaces;

public interface IPositionService
{
    Task<IEnumerable<PositionDto>> GetAllAsync();
    Task<PositionDto?> GetByIdAsync(int id);
    Task<PositionDto> CreateAsync(CreatePositionDto dto);
    Task<PositionDto> UpdateAsync(int id, UpdatePositionDto dto);
    Task<bool> DeleteAsync(int id);
}
