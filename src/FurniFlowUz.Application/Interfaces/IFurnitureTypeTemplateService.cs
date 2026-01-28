using FurniFlowUz.Application.DTOs.FurnitureTypeTemplate;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for furniture type template operations
/// </summary>
public interface IFurnitureTypeTemplateService
{
    /// <summary>
    /// Gets all templates
    /// </summary>
    Task<IEnumerable<FurnitureTypeTemplateDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets template by ID
    /// </summary>
    Task<FurnitureTypeTemplateDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active templates for a category
    /// </summary>
    Task<IEnumerable<FurnitureTypeTemplateDto>> GetActiveByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all templates for a category (including inactive)
    /// </summary>
    Task<IEnumerable<FurnitureTypeTemplateDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new template
    /// </summary>
    Task<FurnitureTypeTemplateDto> CreateAsync(CreateFurnitureTypeTemplateDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing template
    /// </summary>
    Task<FurnitureTypeTemplateDto> UpdateAsync(int id, UpdateFurnitureTypeTemplateDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a template
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles template active status
    /// </summary>
    Task<FurnitureTypeTemplateDto> ToggleActiveStatusAsync(int id, CancellationToken cancellationToken = default);
}
