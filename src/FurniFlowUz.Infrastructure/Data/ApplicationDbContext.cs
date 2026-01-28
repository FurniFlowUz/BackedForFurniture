using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    // DbSet properties for all 26 entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<FurnitureTypeTemplate> FurnitureTypeTemplates => Set<FurnitureTypeTemplate>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<FurnitureType> FurnitureTypes => Set<FurnitureType>();
    public DbSet<Detail> Details => Set<Detail>();
    public DbSet<Drawing> Drawings => Set<Drawing>();
    public DbSet<TechnicalSpecification> TechnicalSpecifications => Set<TechnicalSpecification>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<WorkTask> WorkTasks => Set<WorkTask>();
    public DbSet<ProductionStage> ProductionStages => Set<ProductionStage>();
    public DbSet<WarehouseItem> WarehouseItems => Set<WarehouseItem>();
    public DbSet<WarehouseTransaction> WarehouseTransactions => Set<WarehouseTransaction>();
    public DbSet<MaterialRequest> MaterialRequests => Set<MaterialRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<KPI> KPIs => Set<KPI>();
    public DbSet<CategoryAssignment> CategoryAssignments => Set<CategoryAssignment>();
    public DbSet<DetailTask> DetailTasks => Set<DetailTask>();
    public DbSet<TaskPerformance> TaskPerformances => Set<TaskPerformance>();
    public DbSet<MaterialAssignment> MaterialAssignments => Set<MaterialAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply soft delete global query filter for all BaseAuditableEntity types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));
                var filterExpression = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false)),
                    parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filterExpression);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get the current user ID from ICurrentUserService
        int? currentUserId = _currentUserService?.UserId;

        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;

                if (entry.Entity is BaseAuditableEntity auditableEntity)
                {
                    auditableEntity.CreatedBy = currentUserId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;

                if (entry.Entity is BaseAuditableEntity auditableEntity)
                {
                    auditableEntity.UpdatedBy = currentUserId;
                }
            }
        }

        // Handle soft deletes
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = currentUserId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
