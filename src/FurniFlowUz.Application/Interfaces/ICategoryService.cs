using FurniFlowUz.Application.DTOs.Category;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for furniture category management
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Gets all categories
    /// </summary>
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new category
    /// </summary>
    Task<CategoryDto> CreateAsync(CreateCategoryDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
