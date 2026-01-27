using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Constructor;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for constructor operations (furniture design, details, technical specifications)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Constructor,Director")]
public class ConstructorController : ControllerBase
{
    private readonly IConstructorService _constructorService;
    private readonly ILogger<ConstructorController> _logger;

    public ConstructorController(IConstructorService constructorService, ILogger<ConstructorController> logger)
    {
        _constructorService = constructorService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all orders assigned to the current constructor
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of orders assigned to constructor</returns>
    [HttpGet("orders")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        var orders = await _constructorService.GetOrdersByConstructorAsync(userId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed order information</returns>
    [HttpGet("orders/{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        var orders = await _constructorService.GetOrdersByConstructorAsync(userId, cancellationToken);
        var order = orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound(ApiResponse<OrderDto>.FailureResponse("Order not found or not assigned to you"));
        }

        return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully"));
    }

    /// <summary>
    /// Gets all furniture types for a specific order
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of furniture types for the order</returns>
    [HttpGet("orders/{orderId}/furniture-types")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FurnitureTypeDto>>>> GetFurnitureTypesByOrder(
        [FromRoute] int orderId,
        CancellationToken cancellationToken)
    {
        var furnitureTypes = await _constructorService.GetFurnitureTypesByOrderAsync(orderId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<FurnitureTypeDto>>.SuccessResponse(furnitureTypes, "Furniture types retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific furniture type by ID
    /// </summary>
    /// <param name="id">Furniture type ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Furniture type information</returns>
    [HttpGet("furniture-types/{id}")]
    public async Task<ActionResult<ApiResponse<FurnitureTypeDto>>> GetFurnitureType(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var furnitureType = await _constructorService.GetFurnitureTypeByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<FurnitureTypeDto>.SuccessResponse(furnitureType, "Furniture type retrieved successfully"));
    }

    /// <summary>
    /// Creates a new furniture type for an order
    /// </summary>
    /// <param name="request">Furniture type creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created furniture type information</returns>
    [HttpPost("furniture-types")]
    public async Task<ActionResult<ApiResponse<FurnitureTypeDto>>> CreateFurnitureType(
        [FromBody] CreateFurnitureTypeDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<FurnitureTypeDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var furnitureType = await _constructorService.CreateFurnitureTypeAsync(request, cancellationToken);
        return Ok(ApiResponse<FurnitureTypeDto>.SuccessResponse(furnitureType, "Furniture type created successfully"));
    }

    /// <summary>
    /// Updates an existing furniture type
    /// </summary>
    /// <param name="id">Furniture type ID</param>
    /// <param name="request">Updated furniture type data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated furniture type information</returns>
    [HttpPut("furniture-types/{id}")]
    public async Task<ActionResult<ApiResponse<FurnitureTypeDto>>> UpdateFurnitureType(
        [FromRoute] int id,
        [FromBody] UpdateFurnitureTypeDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<FurnitureTypeDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var furnitureType = await _constructorService.UpdateFurnitureTypeAsync(id, request, cancellationToken);
        return Ok(ApiResponse<FurnitureTypeDto>.SuccessResponse(furnitureType, "Furniture type updated successfully"));
    }

    /// <summary>
    /// Deletes a furniture type
    /// </summary>
    /// <param name="id">Furniture type ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("furniture-types/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFurnitureType(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _constructorService.DeleteFurnitureTypeAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Furniture type deleted successfully"));
    }

    /// <summary>
    /// Gets all details across all furniture types assigned to the current constructor
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all details for the constructor</returns>
    [HttpGet("details")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DetailDto>>>> GetAllConstructorDetails(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        var details = await _constructorService.GetAllConstructorDetailsAsync(userId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<DetailDto>>.SuccessResponse(details, "All constructor details retrieved successfully"));
    }

    /// <summary>
    /// Gets all details for a furniture type
    /// </summary>
    /// <param name="furnitureTypeId">Furniture type ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of details for the furniture type</returns>
    [HttpGet("furniture-types/{furnitureTypeId}/details")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DetailDto>>>> GetDetailsByFurnitureType(
        [FromRoute] int furnitureTypeId,
        CancellationToken cancellationToken)
    {
        var details = await _constructorService.GetDetailsByFurnitureTypeAsync(furnitureTypeId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<DetailDto>>.SuccessResponse(details, "Details retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific detail by ID
    /// </summary>
    /// <param name="id">Detail ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detail information</returns>
    [HttpGet("details/{id}")]
    public async Task<ActionResult<ApiResponse<DetailDto>>> GetDetail(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var detail = await _constructorService.GetDetailByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DetailDto>.SuccessResponse(detail, "Detail retrieved successfully"));
    }

    /// <summary>
    /// Creates a new detail for a furniture type
    /// </summary>
    /// <param name="request">Detail creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created detail information</returns>
    [HttpPost("details")]
    public async Task<ActionResult<ApiResponse<DetailDto>>> CreateDetail(
        [FromBody] CreateDetailDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DetailDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var detail = await _constructorService.CreateDetailAsync(request, cancellationToken);
        return Ok(ApiResponse<DetailDto>.SuccessResponse(detail, "Detail created successfully"));
    }

    /// <summary>
    /// Updates an existing detail
    /// </summary>
    /// <param name="id">Detail ID</param>
    /// <param name="request">Updated detail data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated detail information</returns>
    [HttpPut("details/{id}")]
    public async Task<ActionResult<ApiResponse<DetailDto>>> UpdateDetail(
        [FromRoute] int id,
        [FromBody] UpdateDetailDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<DetailDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var detail = await _constructorService.UpdateDetailAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DetailDto>.SuccessResponse(detail, "Detail updated successfully"));
    }

    /// <summary>
    /// Deletes a detail
    /// </summary>
    /// <param name="id">Detail ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("details/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDetail(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _constructorService.DeleteDetailAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Detail deleted successfully"));
    }

    /// <summary>
    /// Gets all drawings across all furniture types assigned to the current constructor
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all drawings for the constructor</returns>
    [HttpGet("drawings")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DrawingDto>>>> GetAllConstructorDrawings(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        var drawings = await _constructorService.GetAllConstructorDrawingsAsync(userId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<DrawingDto>>.SuccessResponse(drawings, "All constructor drawings retrieved successfully"));
    }

    /// <summary>
    /// Uploads a drawing file for a furniture type
    /// </summary>
    /// <param name="furnitureTypeId">Furniture type ID</param>
    /// <param name="file">Drawing file</param>
    /// <param name="description">Optional description of the drawing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created drawing information</returns>
    [HttpPost("drawings")]
    public async Task<ActionResult<ApiResponse<DrawingDto>>> UploadDrawing(
        [FromForm] int furnitureTypeId,
        [FromForm] IFormFile file,
        [FromForm] string? description,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<DrawingDto>.FailureResponse("File is required"));
        }

        var drawing = await _constructorService.UploadDrawingAsync(furnitureTypeId, file, description, cancellationToken);
        return Ok(ApiResponse<DrawingDto>.SuccessResponse(drawing, "Drawing uploaded successfully"));
    }

    /// <summary>
    /// Deletes a drawing
    /// </summary>
    /// <param name="id">Drawing ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("drawings/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDrawing(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _constructorService.DeleteDrawingAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Drawing deleted successfully"));
    }

    /// <summary>
    /// Creates a technical specification for a furniture type
    /// </summary>
    /// <param name="request">Technical specification creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created technical specification</returns>
    [HttpPost("technical-specifications")]
    public async Task<ActionResult<ApiResponse<TechnicalSpecificationDto>>> CreateTechnicalSpecification(
        [FromBody] CreateTechnicalSpecificationDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<TechnicalSpecificationDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var specification = await _constructorService.CreateTechnicalSpecificationAsync(request, cancellationToken);
        return Ok(ApiResponse<TechnicalSpecificationDto>.SuccessResponse(specification, "Technical specification created successfully"));
    }

    /// <summary>
    /// Completes and locks a technical specification, making it ready for production
    /// </summary>
    /// <param name="furnitureTypeId">Furniture type ID</param>
    /// <param name="request">Completion data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("complete/{furnitureTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteTechnicalSpecification(
        [FromRoute] int furnitureTypeId,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        await _constructorService.CompleteTechnicalSpecificationAsync(furnitureTypeId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Technical specification completed and locked successfully"));
    }
}
