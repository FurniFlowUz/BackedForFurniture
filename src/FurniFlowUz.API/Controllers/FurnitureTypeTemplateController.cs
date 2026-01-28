using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.FurnitureTypeTemplate;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FurnitureTypeTemplateController : ControllerBase
{
    private readonly IFurnitureTypeTemplateService _templateService;
    private readonly ILogger<FurnitureTypeTemplateController> _logger;

    public FurnitureTypeTemplateController(
        IFurnitureTypeTemplateService templateService,
        ILogger<FurnitureTypeTemplateController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Get all furniture type templates
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        try
        {
            var templates = await _templateService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.SuccessResponse(
                templates,
                "Templates retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all templates");
            return StatusCode(500, ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.FailureResponse(
                "An error occurred while retrieving templates"));
        }
    }

    /// <summary>
    /// Get template by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<FurnitureTypeTemplateDto>>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await _templateService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<FurnitureTypeTemplateDto>.SuccessResponse(
                template,
                "Template retrieved successfully"));
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template with ID {TemplateId}", id);
            return StatusCode(500, ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(
                "An error occurred while retrieving the template"));
        }
    }

    /// <summary>
    /// Get all templates for a specific category (including inactive) - for Production Manager
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>>> GetByCategoryId(
        [FromRoute] int categoryId,
        CancellationToken cancellationToken)
    {
        try
        {
            var templates = await _templateService.GetByCategoryIdAsync(categoryId, cancellationToken);
            return Ok(ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.SuccessResponse(
                templates,
                "Templates retrieved successfully"));
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates for category {CategoryId}", categoryId);
            return StatusCode(500, ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.FailureResponse(
                "An error occurred while retrieving templates"));
        }
    }

    /// <summary>
    /// Get active templates for a specific category - for Constructor
    /// </summary>
    [HttpGet("category/{categoryId}/active")]
    [Authorize(Roles = "Constructor,ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>>> GetActiveByCategoryId(
        [FromRoute] int categoryId,
        CancellationToken cancellationToken)
    {
        try
        {
            var templates = await _templateService.GetActiveByCategoryIdAsync(categoryId, cancellationToken);
            return Ok(ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.SuccessResponse(
                templates,
                "Active templates retrieved successfully"));
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active templates for category {CategoryId}", categoryId);
            return StatusCode(500, ApiResponse<IEnumerable<FurnitureTypeTemplateDto>>.FailureResponse(
                "An error occurred while retrieving templates"));
        }
    }

    /// <summary>
    /// Create a new furniture type template
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<FurnitureTypeTemplateDto>>> Create(
        [FromBody] CreateFurnitureTypeTemplateDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        try
        {
            var template = await _templateService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = template.Id },
                ApiResponse<FurnitureTypeTemplateDto>.SuccessResponse(
                    template,
                    "Template created successfully"));
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            return StatusCode(500, ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(
                "An error occurred while creating the template"));
        }
    }

    /// <summary>
    /// Update an existing furniture type template
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<FurnitureTypeTemplateDto>>> Update(
        [FromRoute] int id,
        [FromBody] UpdateFurnitureTypeTemplateDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        try
        {
            var template = await _templateService.UpdateAsync(id, request, cancellationToken);
            return Ok(ApiResponse<FurnitureTypeTemplateDto>.SuccessResponse(
                template,
                "Template updated successfully"));
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template with ID {TemplateId}", id);
            return StatusCode(500, ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(
                "An error occurred while updating the template"));
        }
    }

    /// <summary>
    /// Delete a furniture type template (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _templateService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "Template deleted successfully"));
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template with ID {TemplateId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResponse(
                "An error occurred while deleting the template"));
        }
    }

    /// <summary>
    /// Toggle active status of a furniture type template
    /// </summary>
    [HttpPatch("{id}/toggle-active")]
    [Authorize(Roles = "ProductionManager,Director")]
    public async Task<ActionResult<ApiResponse<FurnitureTypeTemplateDto>>> ToggleActiveStatus(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await _templateService.ToggleActiveStatusAsync(id, cancellationToken);
            return Ok(ApiResponse<FurnitureTypeTemplateDto>.SuccessResponse(
                template,
                "Template active status toggled successfully"));
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling active status for template with ID {TemplateId}", id);
            return StatusCode(500, ApiResponse<FurnitureTypeTemplateDto>.FailureResponse(
                "An error occurred while toggling the template status"));
        }
    }
}
