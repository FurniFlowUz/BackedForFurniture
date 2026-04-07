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
[Authorize(Roles = "Constructor,Director,ProductionManager,TeamLeader")]
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
    /// Gets constructor dashboard statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetStats(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        var orders = await _constructorService.GetOrdersByConstructorAsync(userId, cancellationToken);
        var ordersList = orders.ToList();
        var furnitureTypes = ordersList.SelectMany(o => o.FurnitureTypes ?? new List<FurnitureTypeDto>()).ToList();

        var stats = new
        {
            totalOrders = ordersList.Count,
            activeOrders = ordersList.Count(o => o.Status != "Completed" && o.Status != "Cancelled" && o.Status != "Delivered"),
            completedOrders = ordersList.Count(o => o.Status == "Completed" || o.Status == "Delivered"),
            totalFurnitureTypes = furnitureTypes.Count,
            completedFurnitureTypes = furnitureTypes.Count(ft => ft.TechnicalSpecification?.IsLocked == true),
            pendingFurnitureTypes = furnitureTypes.Count(ft => ft.TechnicalSpecification == null || ft.TechnicalSpecification.IsLocked != true),
            totalDetails = furnitureTypes.Sum(ft => ft.Details?.Count ?? 0),
            totalDrawings = furnitureTypes.Sum(ft => ft.Drawings?.Count ?? 0)
        };

        return Ok(ApiResponse<object>.SuccessResponse(stats, "Constructor stats retrieved successfully"));
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
    /// Force deletes a furniture type even if technical specification is locked
    /// </summary>
    /// <param name="id">Furniture type ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("furniture-types/{id}/force")]
    public async Task<ActionResult<ApiResponse<object>>> ForceDeleteFurnitureType(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _constructorService.ForceDeleteFurnitureTypeAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Furniture type force deleted successfully"));
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

    /// <summary>
    /// Completes a furniture type with all data (details, technical spec) in one request
    /// </summary>
    /// <param name="furnitureTypeId">Furniture type ID</param>
    /// <param name="request">Complete furniture type data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("complete-with-data/{furnitureTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteFurnitureTypeWithData(
        [FromRoute] int furnitureTypeId,
        [FromBody] CompleteFurnitureTypeDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        await _constructorService.CompleteFurnitureTypeWithDataAsync(furnitureTypeId, request, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Furniture type completed successfully with all data"));
    }

    #region Order Images

    /// <summary>
    /// Uploads an image for an order (room photo or design reference)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="imageType">Image type: "room" or "design"</param>
    /// <param name="file">Image file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uploaded image information</returns>
    [HttpPost("order-images")]
    [Authorize(Roles = "Constructor,Director,Salesperson")]
    public async Task<ActionResult<ApiResponse<OrderImageDto>>> UploadOrderImage(
        [FromForm] int orderId,
        [FromForm] string imageType,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<OrderImageDto>.FailureResponse("File is required"));
        }

        if (string.IsNullOrEmpty(imageType) || (imageType != "room" && imageType != "design"))
        {
            return BadRequest(ApiResponse<OrderImageDto>.FailureResponse("Image type must be 'room' or 'design'"));
        }

        var request = new UploadOrderImageRequest
        {
            OrderId = orderId,
            ImageType = imageType,
            File = file
        };

        var result = await _constructorService.UploadOrderImageAsync(request, cancellationToken);
        return Ok(ApiResponse<OrderImageDto>.SuccessResponse(result, "Image uploaded successfully"));
    }

    /// <summary>
    /// Gets all images for a specific order
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of order images</returns>
    [HttpGet("orders/{orderId}/images")]
    [Authorize(Roles = "Constructor,Director,Salesperson")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderImageDto>>>> GetOrderImages(
        [FromRoute] int orderId,
        CancellationToken cancellationToken)
    {
        var images = await _constructorService.GetOrderImagesAsync(orderId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<OrderImageDto>>.SuccessResponse(images, "Images retrieved successfully"));
    }

    /// <summary>
    /// Deletes an order image
    /// </summary>
    /// <param name="imageId">Image ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("order-images/{imageId}")]
    [Authorize(Roles = "Constructor,Director,Salesperson")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteOrderImage(
        [FromRoute] int imageId,
        CancellationToken cancellationToken)
    {
        await _constructorService.DeleteOrderImageAsync(imageId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Image deleted successfully"));
    }

    #endregion
}
