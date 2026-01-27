using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Employee;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for employee management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Director,ProductionManager")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeService employeeService,
        ApplicationDbContext dbContext,
        ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Gets all employees
    /// </summary>
    /// <returns>List of all employees</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetEmployees()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(employees, "Employees retrieved successfully"));
    }

    /// <summary>
    /// Gets all active Team Leaders for category assignment dropdown
    /// Returns lightweight response optimized for dropdown usage
    /// AUTHORIZATION: Only ProductionManager and Director can access
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of team leaders with id and name only</returns>
    /// <response code="200">Returns list of team leaders (empty array if none exist)</response>
    /// <response code="401">Unauthorized - User is not authenticated</response>
    /// <response code="403">Forbidden - User is not ProductionManager or Director</response>
    [HttpGet("team-leaders")]
    [Authorize(Roles = "ProductionManager,Director")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTeamLeaders(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProductionManager requesting team leaders list for assignment");

        var teamLeaders = await _dbContext.Employees
            .Include(e => e.User)
            .Where(e =>
                e.IsActive &&
                e.User.IsActive &&
                e.User.Role == Domain.Enums.UserRole.TeamLeader)
            .OrderBy(e => e.FullName)
            .Select(e => new
            {
                id = e.Id,
                name = e.FullName
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} active team leaders", teamLeaders.Count);

        return Ok(ApiResponse<List<object>>.SuccessResponse(
            teamLeaders.Cast<object>().ToList(),
            teamLeaders.Count > 0
                ? $"{teamLeaders.Count} team leader(s) available"
                : "No team leaders available"
        ));
    }

    /// <summary>
    /// Gets employee by ID
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Employee information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(employee, "Employee retrieved successfully"));
    }

    /// <summary>
    /// Creates a new employee with user account (transactional)
    /// </summary>
    /// <param name="request">Employee creation data</param>
    /// <returns>Created employee information</returns>
    [HttpPost("with-user")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> CreateEmployeeWithUser(
        [FromBody] CreateEmployeeWithUserDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<EmployeeDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var employee = await _employeeService.CreateWithUserAsync(request);
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(employee, "Employee created successfully"));
    }

    /// <summary>
    /// Updates an existing employee
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <param name="request">Employee update data</param>
    /// <returns>Updated employee information</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> UpdateEmployee(
        int id,
        [FromBody] UpdateEmployeeDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<EmployeeDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var employee = await _employeeService.UpdateAsync(id, request);
        return Ok(ApiResponse<EmployeeDto>.SuccessResponse(employee, "Employee updated successfully"));
    }

    /// <summary>
    /// Toggles employee active status
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Success result with new status</returns>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleEmployeeStatus(int id)
    {
        var isActive = await _employeeService.ToggleStatusAsync(id);
        return Ok(ApiResponse<bool>.SuccessResponse(isActive, $"Employee status updated to {(isActive ? "active" : "inactive")}"));
    }

    /// <summary>
    /// Deletes an employee (soft delete)
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(int id)
    {
        var result = await _employeeService.DeleteAsync(id);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Employee deleted successfully"));
    }
}
