using FurniFlowUz.Domain.Entities;

namespace FurniFlowUz.Domain.Interfaces;

/// <summary>
/// Repository interface for FurnitureTypeTemplate operations
/// </summary>
public interface IFurnitureTypeTemplateRepository : IRepository<FurnitureTypeTemplate>
{
    /// <summary>
    /// Gets all active templates for a specific category
    /// </summary>
    Task<IEnumerable<FurnitureTypeTemplate>> GetActiveByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all templates for a specific category (including inactive)
    /// </summary>
    Task<IEnumerable<FurnitureTypeTemplate>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
}
