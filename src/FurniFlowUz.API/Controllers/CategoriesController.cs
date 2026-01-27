using FurniFlowUz.Application.DTOs.Category;
using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for furniture category management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ProductionManager,Director")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryService categoryService,
        ApplicationDbContext dbContext,
        ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all categories</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categories, "Categories retrieved successfully"));
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="request">Category creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category information</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory(
        [FromBody] CreateCategoryDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CategoryDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var category = await _categoryService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, "Category created successfully"));
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Updated category data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category information</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(
        [FromRoute] int id,
        [FromBody] UpdateCategoryDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CategoryDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var category = await _categoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, "Category updated successfully"));
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCategory(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Category deleted successfully"));
    }

    /// <summary>
    /// Gets furniture types (categories) that are available for assignment to team leaders
    /// Returns only those not yet assigned or with Assigned status (pending assignment)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of categories ready for assignment</returns>
    /// <response code="200">Categories retrieved successfully</response>
    [HttpGet("for-assignment")]
    [Authorize(Roles = "ProductionManager,Salesperson,Director")]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryForAssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CategoryForAssignmentDto>>>> GetCategoriesForAssignment(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching categories available for assignment");

        // Get all furniture types with their assignments
        var furnitureTypesWithAssignments = await _dbContext.FurnitureTypes
            .Include(ft => ft.Order)
                .ThenInclude(o => o.Customer)
            .Where(ft => !ft.IsDeleted)
            .Select(ft => new
            {
                FurnitureType = ft,
                Assignment = _dbContext.CategoryAssignments
                    .Where(ca => ca.FurnitureTypeId == ft.Id && !ca.IsDeleted)
                    .OrderByDescending(ca => ca.CreatedAt)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        // Filter: exclude those with InProgress or Completed assignments
        var availableCategories = furnitureTypesWithAssignments
            .Where(x =>
                x.Assignment == null ||
                x.Assignment.Status == Domain.Enums.CategoryAssignmentStatus.Assigned)
            .Select(x => new CategoryForAssignmentDto
            {
                CategoryId = x.FurnitureType.Id,
                CategoryName = x.FurnitureType.Name,
                OrderId = x.FurnitureType.OrderId,
                OrderNumber = x.FurnitureType.Order.OrderNumber,
                CustomerName = x.FurnitureType.Order.Customer.FullName
            })
            .OrderBy(c => c.OrderNumber)
            .ThenBy(c => c.CategoryName)
            .ToList();

        _logger.LogInformation("Found {Count} categories available for assignment", availableCategories.Count);

        return Ok(ApiResponse<List<CategoryForAssignmentDto>>.SuccessResponse(
            availableCategories,
            $"{availableCategories.Count} category(ies) available for assignment"));
    }
}
