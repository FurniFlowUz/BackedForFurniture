using AutoMapper;
using FurniFlowUz.Application.DTOs.Department;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _unitOfWork.Departments.GetAllAsync();
        return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        if (department == null)
        {
            throw new NotFoundException(nameof(Department), id);
        }

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        // Check if department name already exists
        var existingDepartments = await _unitOfWork.Departments.FindAsync(
            d => d.Name.ToLower() == dto.Name.ToLower());

        if (existingDepartments.Any())
        {
            throw new ValidationException($"Department with name '{dto.Name}' already exists.");
        }

        var department = _mapper.Map<Department>(dto);
        await _unitOfWork.Departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        if (department == null)
        {
            throw new NotFoundException(nameof(Department), id);
        }

        // Check if new name already exists (excluding current department)
        var existingDepartments = await _unitOfWork.Departments.FindAsync(
            d => d.Name.ToLower() == dto.Name.ToLower() && d.Id != id);

        if (existingDepartments.Any())
        {
            throw new ValidationException($"Department with name '{dto.Name}' already exists.");
        }

        department.Name = dto.Name;
        department.Description = dto.Description;

        _unitOfWork.Departments.Update(department);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        if (department == null)
        {
            throw new NotFoundException(nameof(Department), id);
        }

        // Check if department is being used by any employees
        var employees = await _unitOfWork.Employees.FindAsync(e => e.DepartmentId == id);
        if (employees.Any())
        {
            throw new ValidationException("Cannot delete department that has employees.");
        }

        _unitOfWork.Departments.Remove(department);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
