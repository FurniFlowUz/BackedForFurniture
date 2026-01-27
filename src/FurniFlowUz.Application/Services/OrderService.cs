using AutoMapper;
using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for order management with role-based data filtering
/// </summary>
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public OrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

        // Apply role-based filtering
        var filteredOrders = await ApplyRoleBasedFilteringAsync(orders, cancellationToken);

        return _mapper.Map<IEnumerable<OrderDto>>(filteredOrders);
    }

    public async Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), id);
        }

        // Apply role-based access control - ensure user can only access orders they're authorized for
        var singleOrderList = new List<Order> { order };
        var filteredOrders = await ApplyRoleBasedFilteringAsync(singleOrderList, cancellationToken);

        if (!filteredOrders.Any())
        {
            // User tried to access an order they're not authorized to see
            throw new UnauthorizedAccessException($"You do not have permission to access order {id}.");
        }

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<PaginatedResult<OrderListDto>> GetPagedAsync(OrderFilterDto filter, CancellationToken cancellationToken = default)
    {
        // Build filter predicate with role-based filtering
        var predicate = await BuildFilterPredicateWithRoleAsync(filter, cancellationToken);

        // Get paginated data with Contract included for complete projection
        var orders = await _unitOfWork.Orders.GetPagedAsync(
            filter.PageNumber,
            filter.PageSize,
            predicate,
            orderBy: q => q.OrderByDescending(o => o.CreatedAt),
            includeProperties: "Customer,Category,Contract,AssignedConstructor,AssignedProductionManager",
            cancellationToken);

        // Count with role-based filtering applied
        // Get all orders and apply role-based filtering to get accurate count
        var allOrders = await _unitOfWork.Orders.FindAsync(predicate, cancellationToken);
        var totalCount = allOrders.Count();

        // Load all categories once for efficient lookup
        var allCategories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        var categoryDict = allCategories.ToDictionary(c => c.Id, c => c.Name);

        // Manual projection with complete display-ready data
        var orderDtos = orders.Select(order => new OrderListDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ContractId = order.ContractId,
            ContractNumber = order.Contract?.ContractNumber,
            CustomerName = order.Customer?.FullName ?? "Unknown",

            // Parse category names from contract
            CategoryNames = order.Contract != null && !string.IsNullOrEmpty(order.Contract.CategoryIds)
                ? order.Contract.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var catId) ? catId : 0)
                    .Where(id => id > 0 && categoryDict.ContainsKey(id))
                    .Select(id => categoryDict[id])
                    .ToList()
                : new List<string> { categoryDict.ContainsKey(order.CategoryId) ? categoryDict[order.CategoryId] : "Unknown" },

            TotalAmount = order.Contract?.TotalAmount ?? 0,
            Status = order.Status.ToString(),
            ProgressPercentage = order.ProgressPercentage,
            AssignedConstructorName = order.AssignedConstructor != null
                ? $"{order.AssignedConstructor.FirstName} {order.AssignedConstructor.LastName}"
                : null,
            AssignedProductionManagerName = order.AssignedProductionManager != null
                ? $"{order.AssignedProductionManager.FirstName} {order.AssignedProductionManager.LastName}"
                : null,
            DeadlineDate = order.DeadlineDate,
            CreatedAt = order.CreatedAt
        }).ToList();

        return new PaginatedResult<OrderListDto>
        {
            Items = orderDtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto request, CancellationToken cancellationToken = default)
    {
        // ===================================================================
        // LOAD AND VALIDATE CONTRACT (REQUIRED)
        // ===================================================================
        var contract = await _unitOfWork.Contracts.GetByIdAsync(request.ContractId, cancellationToken);
        if (contract == null)
        {
            throw new NotFoundException(nameof(Contract), request.ContractId);
        }

        // ===================================================================
        // BUSINESS RULE VALIDATION
        // ===================================================================

        // Contract must be in a valid state for order creation
        if (contract.Status == ContractStatus.Cancelled)
        {
            throw new BusinessException("Cannot create order from a cancelled contract.");
        }

        if (contract.Status == ContractStatus.New && !contract.RequiresApproval)
        {
            throw new BusinessException("Contract must be approved before creating orders.");
        }

        // Optional: Prevent duplicate orders per contract (can be adjusted based on business rules)
        var existingOrders = await _unitOfWork.Orders.FindAsync(
            o => o.ContractId == request.ContractId,
            cancellationToken);

        if (existingOrders.Any())
        {
            throw new BusinessException($"An order already exists for contract '{contract.ContractNumber}'. Only one order per contract is allowed.");
        }

        // ===================================================================
        // DERIVE CUSTOMER AND CATEGORY FROM CONTRACT
        // ===================================================================
        var customerId = contract.CustomerId;

        // Get first category from contract's CategoryIds
        var categoryIds = string.IsNullOrEmpty(contract.CategoryIds)
            ? new List<int>()
            : contract.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var catId) ? catId : 0)
                .Where(id => id > 0)
                .ToList();

        if (!categoryIds.Any())
        {
            throw new BusinessException("Contract has no categories. Cannot create order.");
        }

        var primaryCategoryId = categoryIds.First(); // Use first category as primary

        // Calculate deadline from contract production duration
        var deadlineDate = contract.SignedDate.HasValue
            ? contract.SignedDate.Value.AddDays(contract.ProductionDurationDays)
            : DateTime.UtcNow.AddDays(contract.ProductionDurationDays);

        // ===================================================================
        // GENERATE ORDER NUMBER
        // ===================================================================
        var orderNumber = await GenerateOrderNumberAsync(cancellationToken);

        // ===================================================================
        // CREATE ORDER
        // ===================================================================
        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = customerId,              // ✅ Derived from Contract
            CategoryId = primaryCategoryId,       // ✅ Derived from Contract
            ContractId = contract.Id,
            DeadlineDate = deadlineDate,          // ✅ Calculated from Contract
            Status = OrderStatus.New,
            ProgressPercentage = 0,
            Notes = string.IsNullOrEmpty(request.Notes)
                ? $"Order created from contract {contract.ContractNumber}"
                : request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load customer for notification
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);

        // Send notification to Director
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "New Order Created",
            Message = $"Order {orderNumber} has been created for customer {customer?.FullName ?? "Unknown"}.",
            Type = nameof(NotificationType.OrderStatusChanged),
            Role = UserRole.Director.ToString()
        }, cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> UpdateAsync(int id, UpdateOrderDto request, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), id);
        }

        // Update order properties
        order.DeadlineDate = request.ExpectedDeliveryDate;
        order.CompletedAt = request.ActualDeliveryDate;
        order.Status = Enum.Parse<OrderStatus>(request.Status);
        order.Notes = request.Description; // Description maps to Notes
        order.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }

public async Task AssignConstructorAsync(
    int orderId,
    int constructorId,
    CancellationToken cancellationToken = default)
{
    var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
    if (order == null)
    {
        throw new NotFoundException(nameof(Order), orderId);
    }

    // Constructor mavjudligini tekshirish
    var constructor = await _unitOfWork.Employees.GetByIdAsync(constructorId, cancellationToken);
    if (constructor == null)
    {
        throw new NotFoundException(nameof(Employee), constructorId);
    }

    var userConstructor = await _unitOfWork.Users.GetByIdAsync(
        constructor.UserId,
        cancellationToken);

    if (userConstructor == null)
    {
        throw new NotFoundException(nameof(User), constructor.UserId);
    }

    if (userConstructor.Role != UserRole.Constructor)
    {
        throw new ValidationException("User must have Constructor role.");
    }

    if (!constructor.IsActive)
    {
        throw new ValidationException("Constructor is not active.");
    }

    // ===============================
    // 🔥 FIX: Assign User.Id, not Employee.Id
    // ===============================

    // CRITICAL: Order.AssignedConstructorId is FK to Users table (not Employees)
    // Must assign constructor.UserId (User.Id), not constructorId (Employee.Id)
    order.AssignedConstructorId = constructor.UserId;

    // 🔴 STATUSNI MAJBURIY YANGILAYMIZ
    order.Status = OrderStatus.Assigned;

    order.UpdatedAt = DateTime.UtcNow;

    _unitOfWork.Orders.Update(order);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // (ixtiyoriy) Notification
        await _notificationService.CreateNotificationAsync(
            new CreateNotificationDto
            {
                UserId = constructor.UserId,
                Title = "New Order Assigned",
                Message = $"Order #{order.OrderNumber} has been assigned to you",
                Type = NotificationType.OrderAssigned.ToString()
            },

            cancellationToken
        );
}


    public async Task AssignProductionManagerAsync(int orderId, int productionManagerId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        // ⚠️ CRITICAL: productionManagerId is Employee.Id (NOT User.Id)
        // Step 1: Fetch Employee record
        var employee = await _unitOfWork.Employees.GetByIdAsync(productionManagerId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee", productionManagerId);
        }

        if (!employee.IsActive)
        {
            throw new ValidationException("Production manager employee is not active.");
        }

        // Step 2: Fetch linked User record via employee.UserId
        var productionManager = await _unitOfWork.Users.GetByIdAsync(employee.UserId, cancellationToken);
        if (productionManager == null)
        {
            throw new NotFoundException("User", employee.UserId);
        }

        // Step 3: Validate User has ProductionManager role
        if (productionManager.Role != UserRole.ProductionManager)
        {
            throw new ValidationException("User must have ProductionManager role.");
        }

        if (!productionManager.IsActive)
        {
            throw new ValidationException("Production manager user is not active.");
        }

        // ⚠️ CRITICAL: Assign User.Id (NOT Employee.Id)
        // Order.AssignedProductionManagerId is FK to Users table
        order.AssignedProductionManagerId = employee.UserId;
        order.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to production manager (using User.Id)
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Order Assigned",
            Message = $"Order {order.OrderNumber} has been assigned to you for production management.",
            Type = nameof(NotificationType.TaskAssigned),
            UserId = employee.UserId  // ⚠️ Use User.Id for notifications
        }, cancellationToken);
    }

    public async Task UpdateStatusAsync(int id, OrderStatus status, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), id);
        }

        var oldStatus = order.Status;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        // If order is completed, set completion date
        if (status == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
            order.ProgressPercentage = 100;
        }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notifications based on status change
        if (oldStatus != status)
        {
            string message = status switch
            {
                OrderStatus.SpecificationsReady => $"Order {order.OrderNumber} specifications are ready for production.",
                OrderStatus.InProduction => $"Order {order.OrderNumber} is now in production.",
                OrderStatus.Completed => $"Order {order.OrderNumber} has been completed.",
                OrderStatus.Cancelled => $"Order {order.OrderNumber} has been cancelled.",
                _ => $"Order {order.OrderNumber} status changed to {status}."
            };

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Order Status Updated",
                Message = message,
                Type = nameof(NotificationType.OrderStatusChanged),
                Role = UserRole.Director.ToString()
            }, cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), id);
        }

        // Check if order has associated furniture types or tasks
        var furnitureTypes = await _unitOfWork.FurnitureTypes.FindAsync(f => f.OrderId == id, cancellationToken);
        if (furnitureTypes.Any())
        {
            throw new BusinessException("Cannot delete order with associated furniture types.");
        }

        var tasks = await _unitOfWork.WorkTasks.FindAsync(t => t.OrderId == id, cancellationToken);
        if (tasks.Any())
        {
            throw new BusinessException("Cannot delete order with associated work tasks.");
        }

        _unitOfWork.Orders.Remove(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow;
        var prefix = $"ORD-{today:yyyyMMdd}";

        // Get all order numbers with today's prefix to find the highest sequence
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var todayOrders = allOrders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .ToList();

        int maxSequence = 0;
        if (todayOrders.Any())
        {
            // Extract sequence numbers from existing order numbers
            foreach (var order in todayOrders)
            {
                var parts = order.OrderNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var seq))
                {
                    if (seq > maxSequence)
                    {
                        maxSequence = seq;
                    }
                }
            }
        }

        var sequence = maxSequence + 1;

        return $"{prefix}-{sequence:D4}";
    }

    public async Task<OrderStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

        // Apply role-based filtering
        var filteredOrders = await ApplyRoleBasedFilteringAsync(allOrders, cancellationToken);

        var totalOrders = filteredOrders.Count();

        // Created: New or Assigned status
        var created = filteredOrders.Count(o => o.Status == OrderStatus.New || o.Status == OrderStatus.Assigned);

        // InProgress: SpecificationsReady, InProduction, QualityCheck
        var inProgress = filteredOrders.Count(o =>
            o.Status == OrderStatus.SpecificationsReady ||
            o.Status == OrderStatus.InProduction ||
            o.Status == OrderStatus.QualityCheck);

        // Completed: Completed or Delivered
        var completed = filteredOrders.Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered);

        return new OrderStatsDto
        {
            TotalOrders = totalOrders,
            Created = created,
            InProgress = inProgress,
            Completed = completed
        };
    }

    #region Private Helper Methods

    /// <summary>
    /// Resolves the current user's Employee ID from their User ID
    /// CRITICAL: Constructor/ProductionManager data is linked via Employees table
    /// This method provides centralized UserId → EmployeeId resolution
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee ID if found, null otherwise</returns>
    private async Task<int?> GetCurrentEmployeeIdAsync(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            return null;
        }

        var userId = _currentUserService.UserId.Value;

        var employees = await _unitOfWork.Employees
            .FindAsync(e => e.UserId == userId && e.IsActive, cancellationToken);

        var employee = employees.FirstOrDefault();
        return employee?.Id;
    }

    /// <summary>
    /// Applies role-based filtering to a collection of orders
    /// CRITICAL BUSINESS LOGIC: Filters data based on current user's role
    /// NOTE: For Constructor/ProductionManager roles, this method filters by User.Id
    /// since AssignedConstructorId/AssignedProductionManagerId are FKs to Users table
    /// </summary>
    /// <param name="orders">Orders to filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Filtered orders based on user role</returns>
    private Task<IEnumerable<Order>> ApplyRoleBasedFilteringAsync(
        IEnumerable<Order> orders,
        CancellationToken cancellationToken)
    {
        // If user is not authenticated, return empty list
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            return Task.FromResult(Enumerable.Empty<Order>());
        }

        var currentUserId = _currentUserService.UserId.Value;
        var currentRole = _currentUserService.Role;

        // Parse role from string to enum
        if (!Enum.TryParse<UserRole>(currentRole, out var userRole))
        {
            return Task.FromResult(Enumerable.Empty<Order>());
        }

        // Apply filtering based on role
        IEnumerable<Order> result = userRole switch
        {
            UserRole.Director =>
                // Directors see ALL orders - no filtering
                orders,

            UserRole.Constructor =>
                // Constructors see ONLY orders assigned to them
                // IMPORTANT: AssignedConstructorId stores User.Id (after our fix)
                // Filters orders where AssignedConstructorId matches current User.Id
                orders.Where(o => o.AssignedConstructorId == currentUserId),

            UserRole.ProductionManager =>
                // Production Managers see ONLY orders assigned to them
                // IMPORTANT: AssignedProductionManagerId stores User.Id
                // Filters orders where AssignedProductionManagerId matches current User.Id
                orders.Where(o => o.AssignedProductionManagerId == currentUserId),

            UserRole.Salesperson =>
                // Salespeople see ONLY orders they created
                // CreatedBy is tracked in BaseAuditableEntity (stores User.Id)
                orders.Where(o => o.CreatedBy == currentUserId),

            _ =>
                // Other roles (TeamLeader, Worker, WarehouseManager) see no orders by default
                Enumerable.Empty<Order>()
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Builds filter predicate with role-based filtering applied
    /// Combines user-provided filters with role-based access control
    /// </summary>
    /// <param name="filter">User-provided filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Combined filter predicate</returns>
    private Task<System.Linq.Expressions.Expression<Func<Order, bool>>> BuildFilterPredicateWithRoleAsync(
        OrderFilterDto filter,
        CancellationToken cancellationToken)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Order), "o");
        System.Linq.Expressions.Expression combinedExpression = System.Linq.Expressions.Expression.Constant(true);

        // If user is not authenticated, return false predicate (no results)
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            return Task.FromResult(System.Linq.Expressions.Expression.Lambda<Func<Order, bool>>(
                System.Linq.Expressions.Expression.Constant(false),
                parameter));
        }

        var currentUserId = _currentUserService.UserId.Value;
        var currentRole = _currentUserService.Role;

        // Parse role from string to enum
        if (!Enum.TryParse<UserRole>(currentRole, out var userRole))
        {
            return Task.FromResult(System.Linq.Expressions.Expression.Lambda<Func<Order, bool>>(
                System.Linq.Expressions.Expression.Constant(false),
                parameter));
        }

        // Build role-based filter expression
        System.Linq.Expressions.Expression? roleFilter = null;

        switch (userRole)
        {
            case UserRole.Director:
                // Directors see all - no additional filter needed
                break;

            case UserRole.Constructor:
                // Filter: o.AssignedConstructorId == currentUserId
                var constructorProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(Order.AssignedConstructorId));
                var constructorValue = System.Linq.Expressions.Expression.Constant((int?)currentUserId, typeof(int?));
                roleFilter = System.Linq.Expressions.Expression.Equal(constructorProperty, constructorValue);
                break;

            case UserRole.ProductionManager:
                // Filter: o.AssignedProductionManagerId == currentUserId
                var managerProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(Order.AssignedProductionManagerId));
                var managerValue = System.Linq.Expressions.Expression.Constant((int?)currentUserId, typeof(int?));
                roleFilter = System.Linq.Expressions.Expression.Equal(managerProperty, managerValue);
                break;

            case UserRole.Salesperson:
                // Filter: o.CreatedBy == currentUserId
                var createdByProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(Order.CreatedBy));
                var createdByValue = System.Linq.Expressions.Expression.Constant((int?)currentUserId, typeof(int?));
                roleFilter = System.Linq.Expressions.Expression.Equal(createdByProperty, createdByValue);
                break;

            default:
                // Other roles see nothing
                return Task.FromResult(System.Linq.Expressions.Expression.Lambda<Func<Order, bool>>(
                    System.Linq.Expressions.Expression.Constant(false),
                    parameter));
        }

        // Combine role filter with base expression
        if (roleFilter != null)
        {
            combinedExpression = System.Linq.Expressions.Expression.AndAlso(combinedExpression, roleFilter);
        }

        // TODO: Add additional filters from OrderFilterDto (status, date range, etc.)
        // You can extend this to include filter.Status, filter.DateFrom, etc.

        return Task.FromResult(System.Linq.Expressions.Expression.Lambda<Func<Order, bool>>(combinedExpression, parameter));
    }

    #endregion
}
