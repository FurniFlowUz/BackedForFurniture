using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FurniFlowUz.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    // Lazy-initialized repositories
    private IRepository<User>? _users;
    private IRepository<Employee>? _employees;
    private IRepository<Position>? _positions;
    private IRepository<Department>? _departments;
    private IRepository<Customer>? _customers;
    private IRepository<Category>? _categories;
    private IRepository<Contract>? _contracts;
    private IRepository<Order>? _orders;
    private IRepository<FurnitureType>? _furnitureTypes;
    private IRepository<Detail>? _details;
    private IRepository<Drawing>? _drawings;
    private IRepository<TechnicalSpecification>? _technicalSpecifications;
    private IRepository<Team>? _teams;
    private IRepository<WorkTask>? _workTasks;
    private IRepository<ProductionStage>? _productionStages;
    private IRepository<WarehouseItem>? _warehouseItems;
    private IRepository<WarehouseTransaction>? _warehouseTransactions;
    private IRepository<MaterialRequest>? _materialRequests;
    private IRepository<Notification>? _notifications;
    private IRepository<AuditLog>? _auditLogs;
    private IRepository<KPI>? _kpis;
    private IRepository<CategoryAssignment>? _categoryAssignments;
    private IRepository<DetailTask>? _detailTasks;
    private IRepository<TaskPerformance>? _taskPerformances;
    private IRepository<MaterialAssignment>? _materialAssignments;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization
    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Employee> Employees => _employees ??= new Repository<Employee>(_context);
    public IRepository<Position> Positions => _positions ??= new Repository<Position>(_context);
    public IRepository<Department> Departments => _departments ??= new Repository<Department>(_context);
    public IRepository<Customer> Customers => _customers ??= new Repository<Customer>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<Contract> Contracts => _contracts ??= new Repository<Contract>(_context);
    public IRepository<Order> Orders => _orders ??= new Repository<Order>(_context);
    public IRepository<FurnitureType> FurnitureTypes => _furnitureTypes ??= new Repository<FurnitureType>(_context);
    public IRepository<Detail> Details => _details ??= new Repository<Detail>(_context);
    public IRepository<Drawing> Drawings => _drawings ??= new Repository<Drawing>(_context);
    public IRepository<TechnicalSpecification> TechnicalSpecifications => _technicalSpecifications ??= new Repository<TechnicalSpecification>(_context);
    public IRepository<Team> Teams => _teams ??= new Repository<Team>(_context);
    public IRepository<WorkTask> WorkTasks => _workTasks ??= new Repository<WorkTask>(_context);
    public IRepository<ProductionStage> ProductionStages => _productionStages ??= new Repository<ProductionStage>(_context);
    public IRepository<WarehouseItem> WarehouseItems => _warehouseItems ??= new Repository<WarehouseItem>(_context);
    public IRepository<WarehouseTransaction> WarehouseTransactions => _warehouseTransactions ??= new Repository<WarehouseTransaction>(_context);
    public IRepository<MaterialRequest> MaterialRequests => _materialRequests ??= new Repository<MaterialRequest>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);
    public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
    public IRepository<KPI> KPIs => _kpis ??= new Repository<KPI>(_context);
    public IRepository<CategoryAssignment> CategoryAssignments => _categoryAssignments ??= new Repository<CategoryAssignment>(_context);
    public IRepository<DetailTask> DetailTasks => _detailTasks ??= new Repository<DetailTask>(_context);
    public IRepository<TaskPerformance> TaskPerformances => _taskPerformances ??= new Repository<TaskPerformance>(_context);
    public IRepository<MaterialAssignment> MaterialAssignments => _materialAssignments ??= new Repository<MaterialAssignment>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Handle concurrency conflicts
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is Domain.Common.BaseEntity)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                    if (databaseValues == null)
                    {
                        // Entity was deleted by another user
                        throw new InvalidOperationException(
                            "The entity you attempted to update was deleted by another user.", ex);
                    }

                    // Refresh the entity with database values
                    entry.OriginalValues.SetValues(databaseValues);

                    // Re-throw to let the caller handle the conflict
                    throw new InvalidOperationException(
                        "The entity you attempted to update was modified by another user. Please refresh and try again.", ex);
                }
            }

            throw;
        }
        catch (DbUpdateException ex)
        {
            // Handle other database update exceptions
            throw new InvalidOperationException(
                "An error occurred while saving changes to the database. See inner exception for details.", ex);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
