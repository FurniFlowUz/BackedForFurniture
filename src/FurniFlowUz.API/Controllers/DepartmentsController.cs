using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Department;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for department management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Director,ProductionManager")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
    {
        _departmentService = departmentService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all departments
    /// </summary>
    /// <returns>List of all departments</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetDepartments()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<DepartmentDto>>.SuccessResponse(departments, "Departments retrieved successfully"));
    }

    /// <summary>
    /// Gets department by ID
    /// </summary>
    /// <param name="id">Department ID</param>
    /// <returns>Department information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartment(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        return Ok(ApiResponse<DepartmentDto>.SuccessResponse(department, "Department retrieved successfully"));
    }

    /// <summary>
    /// Creates a new department
    /// </summary>
    /// <param name="request">Department creation data</param>
    /// <returns>Created department information</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> CreateDepartment(
        [FromBody] CreateDepartmentDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DepartmentDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var department = await _departmentService.CreateAsync(request);
        return Ok(ApiResponse<DepartmentDto>.SuccessResponse(department, "Department created successfully"));
    }

    /// <summary>
    /// Updates an existing department
    /// </summary>
    /// <param name="id">Department ID</param>
    /// <param name="request">Department update data</param>
    /// <returns>Updated department information</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> UpdateDepartment(
        int id,
        [FromBody] UpdateDepartmentDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DepartmentDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var department = await _departmentService.UpdateAsync(id, request);
        return Ok(ApiResponse<DepartmentDto>.SuccessResponse(department, "Department updated successfully"));
    }

    /// <summary>
    /// Deletes a department
    /// </summary>
    /// <param name="id">Department ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteDepartment(int id)
    {
        var result = await _departmentService.DeleteAsync(id);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Department deleted successfully"));
    }
}
