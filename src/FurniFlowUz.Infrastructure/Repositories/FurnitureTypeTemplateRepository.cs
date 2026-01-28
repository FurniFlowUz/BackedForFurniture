using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for FurnitureTypeTemplate operations
/// </summary>
public class FurnitureTypeTemplateRepository : Repository<FurnitureTypeTemplate>, IFurnitureTypeTemplateRepository
{
    public FurnitureTypeTemplateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<FurnitureTypeTemplate>> GetActiveByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Where(t => t.CategoryId == categoryId && t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FurnitureTypeTemplate>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Where(t => t.CategoryId == categoryId && !t.IsDeleted)
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }
}
