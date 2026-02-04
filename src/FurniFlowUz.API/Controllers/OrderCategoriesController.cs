using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.OrderCategory;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for managing Order-Category relationships (Many-to-Many)
/// </summary>
[ApiController]
[Route("api/order-categories")]
[Authorize]
public class OrderCategoriesController : ControllerBase
{
    private readonly IOrderCategoryService _service;
    private readonly ILogger<OrderCategoriesController> _logger;

    public OrderCategoriesController(
        IOrderCategoryService service,
        ILogger<OrderCategoriesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all categories for a specific order
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderCategoryDto>>>> GetByOrderId(
        [FromRoute] int orderId,
        CancellationToken cancellationToken)
    {
        var categories = await _service.GetByOrderIdAsync(orderId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<OrderCategoryDto>>.SuccessResponse(
            categories, "Order categories retrieved successfully"));
    }

    /// <summary>
    /// Gets all orders for a specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderCategoryDto>>>> GetByCategoryId(
        [FromRoute] int categoryId,
        CancellationToken cancellationToken)
    {
        var orders = await _service.GetByCategoryIdAsync(categoryId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<OrderCategoryDto>>.SuccessResponse(
            orders, "Category orders retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific order-category relationship by ID
    /// </summary>
    /// <param name="id">OrderCategory ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderCategoryDto>>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var orderCategory = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<OrderCategoryDto>.SuccessResponse(
            orderCategory, "Order category retrieved successfully"));
    }

    /// <summary>
    /// Adds a category to an order
    /// </summary>
    /// <param name="request">Create request with OrderId and CategoryId</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    [Authorize(Roles = "Director,Seller,ProductionManager")]
    public async Task<ActionResult<ApiResponse<OrderCategoryDto>>> AddCategoryToOrder(
        [FromBody] CreateOrderCategoryDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderCategoryDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var orderCategory = await _service.AddCategoryToOrderAsync(request, cancellationToken);
        return Ok(ApiResponse<OrderCategoryDto>.SuccessResponse(
            orderCategory, "Category added to order successfully"));
    }

    /// <summary>
    /// Sets all categories for an order (replaces existing categories)
    /// </summary>
    /// <param name="request">Bulk request with OrderId and CategoryIds list</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("bulk")]
    [Authorize(Roles = "Director,Seller,ProductionManager")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderCategoryDto>>>> SetOrderCategories(
        [FromBody] BulkOrderCategoryDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<IEnumerable<OrderCategoryDto>>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var orderCategories = await _service.SetOrderCategoriesAsync(request, cancellationToken);
        return Ok(ApiResponse<IEnumerable<OrderCategoryDto>>.SuccessResponse(
            orderCategories, "Order categories updated successfully"));
    }

    /// <summary>
    /// Removes a category from an order
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="categoryId">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("order/{orderId}/category/{categoryId}")]
    [Authorize(Roles = "Director,Seller,ProductionManager")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveCategoryFromOrder(
        [FromRoute] int orderId,
        [FromRoute] int categoryId,
        CancellationToken cancellationToken)
    {
        await _service.RemoveCategoryFromOrderAsync(orderId, categoryId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Category removed from order successfully"));
    }

    /// <summary>
    /// Deletes an order-category relationship by ID
    /// </summary>
    /// <param name="id">OrderCategory ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Director,Seller,ProductionManager")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Order category deleted successfully"));
    }
}
