using System.Linq.Expressions;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Infrastructure.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its ID with eager loading of navigation properties
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="includeProperties">Comma-separated list of navigation properties to include</param>
    Task<T?> GetByIdWithIncludesAsync(int id, string includeProperties, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities that match the specified predicate
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Updates multiple entities
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Removes an entity
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Removes multiple entities
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Gets the total count of entities
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the specified predicate
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated entities with optional filtering, ordering, and eager loading
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="filter">Optional filter predicate</param>
    /// <param name="orderBy">Optional ordering function</param>
    /// <param name="includeProperties">Comma-separated list of navigation properties to include</param>
    Task<IEnumerable<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities that match the specified predicate, ignoring global query filters (including soft delete)
    /// </summary>
    Task<IEnumerable<T>> FindIgnoringQueryFiltersAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
