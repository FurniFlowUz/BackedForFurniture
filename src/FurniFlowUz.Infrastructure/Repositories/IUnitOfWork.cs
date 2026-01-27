using FurniFlowUz.Domain.Entities;

namespace FurniFlowUz.Infrastructure.Repositories;

public interface IUnitOfWork : IDisposable
{
    // Repository properties for all 25 entities
    IRepository<User> Users { get; }
    IRepository<Employee> Employees { get; }
    IRepository<Position> Positions { get; }
    IRepository<Department> Departments { get; }
    IRepository<Customer> Customers { get; }
    IRepository<Category> Categories { get; }
    IRepository<Contract> Contracts { get; }
    IRepository<Order> Orders { get; }
    IRepository<FurnitureType> FurnitureTypes { get; }
    IRepository<Detail> Details { get; }
    IRepository<Drawing> Drawings { get; }
    IRepository<TechnicalSpecification> TechnicalSpecifications { get; }
    IRepository<Team> Teams { get; }
    IRepository<WorkTask> WorkTasks { get; }
    IRepository<ProductionStage> ProductionStages { get; }
    IRepository<WarehouseItem> WarehouseItems { get; }
    IRepository<WarehouseTransaction> WarehouseTransactions { get; }
    IRepository<MaterialRequest> MaterialRequests { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<KPI> KPIs { get; }
    IRepository<CategoryAssignment> CategoryAssignments { get; }
    IRepository<DetailTask> DetailTasks { get; }
    IRepository<TaskPerformance> TaskPerformances { get; }
    IRepository<MaterialAssignment> MaterialAssignments { get; }

    /// <summary>
    /// Saves all changes made in the context to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
