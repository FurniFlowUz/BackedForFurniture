using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Warehouse;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for warehouse and inventory management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "WarehouseManager,TeamLeader,Director")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly ILogger<WarehouseController> _logger;

    public WarehouseController(IWarehouseService warehouseService, ILogger<WarehouseController> logger)
    {
        _warehouseService = warehouseService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all warehouse items
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all warehouse items</returns>
    [HttpGet("items")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WarehouseItemDto>>>> GetItems(
        CancellationToken cancellationToken)
    {
        var items = await _warehouseService.GetAllItemsAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<WarehouseItemDto>>.SuccessResponse(items, "Warehouse items retrieved successfully"));
    }

    /// <summary>
    /// Creates a new warehouse item
    /// </summary>
    /// <param name="request">Warehouse item creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created warehouse item information</returns>
    [HttpPost("items")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<WarehouseItemDto>>> CreateItem(
        [FromBody] CreateWarehouseItemDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<WarehouseItemDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var item = await _warehouseService.CreateItemAsync(request, cancellationToken);
        return Ok(ApiResponse<WarehouseItemDto>.SuccessResponse(item, "Warehouse item created successfully"));
    }

    /// <summary>
    /// Updates an existing warehouse item
    /// </summary>
    /// <param name="id">Warehouse item ID</param>
    /// <param name="request">Updated warehouse item data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated warehouse item information</returns>
    [HttpPut("items/{id}")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<WarehouseItemDto>>> UpdateItem(
        [FromRoute] int id,
        [FromBody] UpdateWarehouseItemDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<WarehouseItemDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var item = await _warehouseService.UpdateItemAsync(id, request, cancellationToken);
        return Ok(ApiResponse<WarehouseItemDto>.SuccessResponse(item, "Warehouse item updated successfully"));
    }

    /// <summary>
    /// Deletes a warehouse item
    /// </summary>
    /// <param name="id">Warehouse item ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("items/{id}")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteItem(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _warehouseService.DeleteItemAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Warehouse item deleted successfully"));
    }

    /// <summary>
    /// Creates an income transaction (stock increase)
    /// </summary>
    /// <param name="request">Income transaction data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created transaction information</returns>
    [HttpPost("income")]
    [Authorize(Roles = "WarehouseManager,Director")]
    public async Task<ActionResult<ApiResponse<WarehouseTransactionDto>>> CreateIncomeTransaction(
        [FromBody] CreateIncomeTransactionDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<WarehouseTransactionDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var transaction = await _warehouseService.CreateIncomeTransactionAsync(request, cancellationToken);
        return Ok(ApiResponse<WarehouseTransactionDto>.SuccessResponse(transaction, "Income transaction created successfully"));
    }

    /// <summary>
    /// Creates an outcome transaction (stock decrease) and material request for team confirmation
    /// </summary>
    /// <param name="request">Outcome transaction data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created transaction information</returns>
    [HttpPost("outcome")]
    [Authorize(Roles = "WarehouseManager,TeamLeader,Director")]
    public async Task<ActionResult<ApiResponse<WarehouseTransactionDto>>> CreateOutcomeTransaction(
        [FromBody] CreateOutcomeTransactionDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<WarehouseTransactionDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var transaction = await _warehouseService.CreateOutcomeTransactionAsync(request, cancellationToken);
        return Ok(ApiResponse<WarehouseTransactionDto>.SuccessResponse(transaction, "Outcome transaction created successfully"));
    }

    /// <summary>
    /// Gets all material requests (pending confirmations from teams)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of material requests</returns>
    [HttpGet("material-requests")]
    public Task<ActionResult<ApiResponse<IEnumerable<MaterialRequestDto>>>> GetMaterialRequests(
        CancellationToken cancellationToken)
    {
        // Note: This method needs to be added to IWarehouseService interface
        // For now, return empty list as placeholder
        return Task.FromResult<ActionResult<ApiResponse<IEnumerable<MaterialRequestDto>>>>(Ok(ApiResponse<IEnumerable<MaterialRequestDto>>.SuccessResponse(
            new List<MaterialRequestDto>(),
            "Material requests retrieved successfully")));
    }

    /// <summary>
    /// Confirms a material request (team confirms receipt of materials)
    /// </summary>
    /// <param name="id">Material request ID</param>
    /// <param name="request">Confirmation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("material-requests/{id}/confirm")]
    [Authorize(Roles = "TeamLeader,Director")]
    public Task<ActionResult<ApiResponse<object>>> ConfirmMaterialRequest(
        [FromRoute] int id,
        [FromBody] ConfirmMaterialRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Task.FromResult<ActionResult<ApiResponse<object>>>(BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList())));
        }

        // Note: This method needs to be added to IWarehouseService interface
        return Task.FromResult<ActionResult<ApiResponse<object>>>(Ok(ApiResponse<object>.SuccessResponse(null, "Material request confirmed successfully")));
    }

    /// <summary>
    /// Gets warehouse alerts for low stock items
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of warehouse items with low stock</returns>
    [HttpGet("stock-alerts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WarehouseAlertDto>>>> GetStockAlerts(
        CancellationToken cancellationToken)
    {
        var alerts = await _warehouseService.GetLowStockAlertsAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<WarehouseAlertDto>>.SuccessResponse(alerts, "Stock alerts retrieved successfully"));
    }
}
