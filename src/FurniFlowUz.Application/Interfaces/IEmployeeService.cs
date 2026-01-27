using FurniFlowUz.Application.DTOs.Employee;

namespace FurniFlowUz.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateWithUserAsync(CreateEmployeeWithUserDto dto);
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto);
    Task<bool> ToggleStatusAsync(int id);
    Task<bool> DeleteAsync(int id);
}
