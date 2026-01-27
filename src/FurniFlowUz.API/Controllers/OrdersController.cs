using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for order management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Salesperson,ProductionManager,Constructor,Director")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Gets paginated list of orders with filtering
    /// </summary>
    /// <param name="filter">Filter parameters for orders</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of orders with complete display-ready data</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<OrderListDto>>>> GetOrders(
        [FromQuery] OrderFilterDto filter,
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetPagedAsync(filter, cancellationToken);
        return Ok(ApiResponse<PaginatedResult<OrderListDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific order by ID with all details
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed order information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully"));
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="request">Order creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order information</returns>
    [HttpPost]
    [Authorize(Roles = "Salesperson,Director")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder(
        [FromBody] CreateOrderDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var order = await _orderService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id },
            ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
    }

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="request">Updated order data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated order information</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Salesperson,Director")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrder(
        [FromRoute] int id,
        [FromBody] UpdateOrderDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var order = await _orderService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order updated successfully"));
    }

    /// <summary>
    /// Assigns a constructor to an order
    /// </summary>
    /// <param name="orderId">Order ID from route</param>
    /// <param name="request">Assignment data containing constructor ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    /// <response code="200">Constructor assigned successfully</response>
    /// <response code="400">Invalid order ID or constructor ID</response>
    /// <response code="403">Forbidden - Only Director or Salesperson can assign constructors</response>
    /// <response code="404">Order or Constructor not found</response>
    [HttpPost("{orderId}/assign-constructor")]
    [Authorize(Roles = "Director,Salesperson")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> AssignConstructor(
        [FromRoute] int orderId,
        [FromBody] AssignConstructorDto request,
        CancellationToken cancellationToken)
    {
        // Validate orderId from route
        if (orderId <= 0)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Order ID must be greater than 0"));
        }

        // Validate request body
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        // Validate constructor ID
        if (request.ConstructorId <= 0)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Constructor ID must be greater than 0"));
        }

        await _orderService.AssignConstructorAsync(orderId, request.ConstructorId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Constructor assigned successfully"));
    }

    /// <summary>
    /// Assigns a production manager to an order
    /// </summary>
    /// <param name="orderId">Order ID from route</param>
    /// <param name="request">Assignment data containing production manager ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    /// <response code="200">Production manager assigned successfully</response>
    /// <response code="400">Invalid order ID or production manager ID</response>
    /// <response code="403">Forbidden - Only Director, ProductionManager, or Salesperson can assign production managers</response>
    /// <response code="404">Order or Production Manager not found</response>
    [HttpPost("{orderId}/assign-production-manager")]
    [Authorize(Roles = "Director,ProductionManager,Salesperson")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> AssignProductionManager(
        [FromRoute] int orderId,
        [FromBody] AssignProductionManagerDto request,
        CancellationToken cancellationToken)
    {
        // Validate orderId from route
        if (orderId <= 0)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Order ID must be greater than 0"));
        }

        // Validate request body
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        // Validate production manager ID
        if (request.ProductionManagerId <= 0)
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Production Manager ID must be greater than 0"));
        }

        await _orderService.AssignProductionManagerAsync(orderId, request.ProductionManagerId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Production manager assigned successfully"));
    }

    /// <summary>
    /// Deletes an order (soft delete)
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteOrder(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _orderService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Order deleted successfully"));
    }

    /// <summary>
    /// Gets order statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order statistics including total, created, in progress, and completed counts</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<OrderStatsDto>>> GetOrderStats(
        CancellationToken cancellationToken)
    {
        var stats = await _orderService.GetStatsAsync(cancellationToken);
        return Ok(ApiResponse<OrderStatsDto>.SuccessResponse(stats, "Order statistics retrieved successfully"));
    }
}
