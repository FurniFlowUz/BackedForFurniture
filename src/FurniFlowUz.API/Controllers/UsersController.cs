using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.User;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for user-related operations
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all available constructors for order assignment
    /// Returns only active users with Constructor role
    /// </summary>
    /// <returns>List of constructors with ID, full name, and phone</returns>
    /// <response code="200">Returns the list of available constructors (empty list if none)</response>
    [HttpGet("constructors")]
    [ProducesResponseType(typeof(ApiResponse<List<ConstructorDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ConstructorDto>>>> GetConstructors()
    {
        var constructors = await _userService.GetAvailableConstructorsAsync();
        return Ok(ApiResponse<List<ConstructorDto>>.SuccessResponse(
            constructors,
            constructors.Count > 0
                ? $"{constructors.Count} constructor(s) available"
                : "No constructors available"
        ));
    }

    /// <summary>
    /// Gets all available production managers for order assignment
    /// Returns only active users with ProductionManager role
    /// </summary>
    /// <returns>List of production managers with ID, full name, and phone</returns>
    /// <response code="200">Returns the list of available production managers (empty list if none)</response>
    [HttpGet("production-managers")]
    [ProducesResponseType(typeof(ApiResponse<List<ConstructorDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ConstructorDto>>>> GetProductionManagers()
    {
        var managers = await _userService.GetAvailableProductionManagersAsync();
        return Ok(ApiResponse<List<ConstructorDto>>.SuccessResponse(
            managers,
            managers.Count > 0
                ? $"{managers.Count} production manager(s) available"
                : "No production managers available"
        ));
    }

    /// <summary>
    /// Gets all available team leaders for task assignment (basic info)
    /// Returns only active users with TeamLeader role
    /// </summary>
    /// <returns>List of team leaders with ID, full name, and phone</returns>
    /// <response code="200">Returns the list of available team leaders (empty list if none)</response>
    [HttpGet("team-leaders")]
    [ProducesResponseType(typeof(ApiResponse<List<TeamLeaderDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TeamLeaderDto>>>> GetTeamLeaders()
    {
        var teamLeaders = await _userService.GetTeamLeadersWithStatsAsync();
        return Ok(ApiResponse<List<TeamLeaderDto>>.SuccessResponse(
            teamLeaders,
            teamLeaders.Count > 0
                ? $"{teamLeaders.Count} team leader(s) available"
                : "No team leaders available"
        ));
    }

    /// <summary>
    /// Gets all furniture types for constructor view
    /// Returns furniture types with summary information including counts
    /// </summary>
    /// <returns>List of furniture types with details and drawing counts</returns>
    /// <response code="200">Returns the list of furniture types (empty list if none)</response>
    [HttpGet("furniture-types")]
    [ProducesResponseType(typeof(ApiResponse<List<FurnitureTypeListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<FurnitureTypeListDto>>>> GetFurnitureTypes()
    {
        var furnitureTypes = await _userService.GetFurnitureTypesAsync();
        return Ok(ApiResponse<List<FurnitureTypeListDto>>.SuccessResponse(
            furnitureTypes,
            furnitureTypes.Count > 0
                ? $"{furnitureTypes.Count} furniture type(s) found"
                : "No furniture types available"
        ));
    }

    /// <summary>
    /// Gets a specific furniture type by ID
    /// Returns furniture type with summary information including counts
    /// </summary>
    /// <param name="id">Furniture type ID</param>
    /// <returns>Furniture type details</returns>
    /// <response code="200">Returns the furniture type details</response>
    /// <response code="404">Furniture type not found</response>
    [HttpGet("furniture-types/{id}")]
    [ProducesResponseType(typeof(ApiResponse<FurnitureTypeListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FurnitureTypeListDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FurnitureTypeListDto>>> GetFurnitureTypeById(int id)
    {
        var furnitureType = await _userService.GetFurnitureTypeByIdAsync(id);
        if (furnitureType == null)
        {
            return NotFound(ApiResponse<FurnitureTypeListDto>.FailureResponse(
                $"Furniture type with ID {id} not found"
            ));
        }

        return Ok(ApiResponse<FurnitureTypeListDto>.SuccessResponse(
            furnitureType,
            "Furniture type retrieved successfully"
        ));
    }

    /// <summary>
    /// Gets orders assigned to the current user as constructor
    /// Returns orders with summary information sorted by deadline
    /// </summary>
    /// <returns>List of assigned orders</returns>
    /// <response code="200">Returns the list of assigned orders (empty list if none)</response>
    [HttpGet("orders-assigned-constructor")]
    [ProducesResponseType(typeof(ApiResponse<List<AssignedOrderDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AssignedOrderDto>>>> GetOrdersAssignedToConstructor()
    {
        var orders = await _userService.GetOrdersAssignedToConstructorAsync();
        return Ok(ApiResponse<List<AssignedOrderDto>>.SuccessResponse(
            orders,
            orders.Count > 0
                ? $"{orders.Count} order(s) assigned to you as constructor"
                : "No orders assigned to you as constructor"
        ));
    }

    /// <summary>
    /// Gets orders assigned to the current user as production manager
    /// Returns orders with summary information sorted by deadline
    /// </summary>
    /// <returns>List of assigned orders</returns>
    /// <response code="200">Returns the list of assigned orders (empty list if none)</response>
    [HttpGet("orders-assigned-production-manager")]
    [ProducesResponseType(typeof(ApiResponse<List<AssignedOrderDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AssignedOrderDto>>>> GetOrdersAssignedToProductionManager()
    {
        var orders = await _userService.GetOrdersAssignedToProductionManagerAsync();
        return Ok(ApiResponse<List<AssignedOrderDto>>.SuccessResponse(
            orders,
            orders.Count > 0
                ? $"{orders.Count} order(s) assigned to you as production manager"
                : "No orders assigned to you as production manager"
        ));
    }

    /// <summary>
    /// Creates a new ProductionManager user account
    /// Creates both User (for authentication) and Employee (for assignment) records
    /// AUTHORIZATION: Only Director role can create ProductionManager accounts
    /// </summary>
    /// <param name="request">ProductionManager creation request</param>
    /// <returns>Created ProductionManager information</returns>
    /// <response code="201">ProductionManager account created successfully</response>
    /// <response code="400">Invalid request data or email already exists</response>
    /// <response code="403">Forbidden - Only Director can create ProductionManager accounts</response>
    [HttpPost("production-managers")]
    [Authorize(Roles = "Director")]  // ⚠️ CRITICAL: ONLY Director can create ProductionManager
    [ProducesResponseType(typeof(ApiResponse<ProductionManagerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProductionManagerDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProductionManagerDto>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ProductionManagerDto>>> CreateProductionManager(
        [FromBody] CreateProductionManagerDto request)
    {
        _logger.LogInformation("Director creating new ProductionManager account for email: {Email}", request.Email);

        var productionManager = await _userService.CreateProductionManagerAsync(request);

        _logger.LogInformation(
            "ProductionManager account created successfully. UserId: {UserId}, EmployeeId: {EmployeeId}",
            productionManager.UserId,
            productionManager.EmployeeId);

        return CreatedAtAction(
            nameof(GetProductionManagers),
            null,
            ApiResponse<ProductionManagerDto>.SuccessResponse(
                productionManager,
                $"ProductionManager account created successfully for {productionManager.Email}"
            ));
    }
}
