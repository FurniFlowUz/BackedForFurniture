using AutoMapper;
using FurniFlowUz.Application.DTOs.Employee;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _unitOfWork.Employees.GetPagedAsync(
            pageNumber: 1,
            pageSize: 10000,
            includeProperties: "User,Position,Department");

        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employees = await _unitOfWork.Employees.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: e => e.Id == id,
            includeProperties: "User,Position,Department");

        var employee = employees.FirstOrDefault();
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), id);
        }

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> CreateWithUserAsync(CreateEmployeeWithUserDto dto)
    {
        // Check if username already exists
        var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == dto.Username);
        if (existingUser.Any())
        {
            throw new ValidationException($"Username '{dto.Username}' already exists.");
        }

        // Check if position exists
        var position = await _unitOfWork.Positions.GetByIdAsync(dto.PositionId);
        if (position == null)
        {
            throw new NotFoundException(nameof(Position), dto.PositionId);
        }

        // Check if department exists
        var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
        if (department == null)
        {
            throw new NotFoundException(nameof(Department), dto.DepartmentId);
        }

        // Parse role
        if (!Enum.TryParse<UserRole>(dto.Role, out var userRole))
        {
            throw new ValidationException($"Invalid role: {dto.Role}");
        }

        // Create user
        var user = new User
        {
            FirstName = dto.FullName.Split(' ')[0],
            LastName = dto.FullName.Contains(' ') ? string.Join(" ", dto.FullName.Split(' ').Skip(1)) : "",
            Email = dto.Username,
            PhoneNumber = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = userRole,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Create employee
        var employee = new Employee
        {
            UserId = user.Id,
            FullName = dto.FullName,
            Phone = dto.Phone,
            PositionId = dto.PositionId,
            DepartmentId = dto.DepartmentId,
            IsActive = true
        };

        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var createdEmployees = await _unitOfWork.Employees.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: e => e.Id == employee.Id,
            includeProperties: "User,Position,Department");

        return _mapper.Map<EmployeeDto>(createdEmployees.First());
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), id);
        }

        // Check if position exists
        var position = await _unitOfWork.Positions.GetByIdAsync(dto.PositionId);
        if (position == null)
        {
            throw new NotFoundException(nameof(Position), dto.PositionId);
        }

        // Check if department exists
        var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
        if (department == null)
        {
            throw new NotFoundException(nameof(Department), dto.DepartmentId);
        }

        employee.FullName = dto.FullName;
        employee.Phone = dto.Phone;
        employee.PositionId = dto.PositionId;
        employee.DepartmentId = dto.DepartmentId;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        // Reload with includes
        var updatedEmployees = await _unitOfWork.Employees.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: e => e.Id == id,
            includeProperties: "User,Position,Department");

        return _mapper.Map<EmployeeDto>(updatedEmployees.First());
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var employees = await _unitOfWork.Employees.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: e => e.Id == id,
            includeProperties: "User");

        var employee = employees.FirstOrDefault();
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), id);
        }

        employee.IsActive = !employee.IsActive;
        employee.User.IsActive = employee.IsActive;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return employee.IsActive;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), id);
        }

        _unitOfWork.Employees.Remove(employee);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
