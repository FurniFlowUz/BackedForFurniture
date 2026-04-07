using AutoMapper;
using FurniFlowUz.Application.DTOs.Seller;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Data;
using FurniFlowUz.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for seller/salesperson-specific operations
/// </summary>
public class SellerService : ISellerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SellerService(
        IUnitOfWork unitOfWork,
        ApplicationDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return userId;
    }

    /// <summary>
    /// Gets dashboard statistics for the seller using EFFICIENT AGGREGATE QUERIES
    /// NO GetAllAsync() - uses CountAsync, SumAsync, GroupBy
    /// NO CancellationToken - avoids TaskCanceledException
    /// </summary>
    public async Task<ContractStatsDto> GetDashboardStatsAsync()
    {
        var userId = GetCurrentUserId();

        // ===================================================================
        // EFFICIENT AGGREGATE QUERIES - NO GETALLASYNC()
        // ===================================================================

        // Count active contracts for this seller
        var activeContracts = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId && c.Status == ContractStatus.Active)
            .CountAsync();

        // Get contract IDs for this seller (needed for orders query)
        var userContractIds = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId)
            .Select(c => c.Id)
            .ToListAsync();

        // Count pending orders (New, Assigned, InProduction)
        var pendingOrders = await _dbContext.Orders
            .Where(o => o.ContractId.HasValue && userContractIds.Contains(o.ContractId.Value))
            .Where(o => o.Status == OrderStatus.New ||
                       o.Status == OrderStatus.Assigned ||
                       o.Status == OrderStatus.InProduction)
            .CountAsync();

        // Count completed orders (Completed, Delivered)
        var completedOrders = await _dbContext.Orders
            .Where(o => o.ContractId.HasValue && userContractIds.Contains(o.ContractId.Value))
            .Where(o => o.Status == OrderStatus.Completed ||
                       o.Status == OrderStatus.Delivered)
            .CountAsync();

        // Sum total revenue from non-cancelled contracts
        var totalRevenue = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId && c.Status != ContractStatus.Cancelled)
            .SumAsync(c => (decimal?)c.TotalAmount) ?? 0;

        // ===================================================================
        // PERIOD COMPARISON (Last 30 days vs Previous 30 days)
        // ===================================================================
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var sixtyDaysAgo = DateTime.UtcNow.AddDays(-60);

        // Current period active contracts (last 30 days)
        var currentPeriodContracts = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId &&
                       c.CreatedAt >= thirtyDaysAgo &&
                       c.Status == ContractStatus.Active)
            .CountAsync();

        // Previous period active contracts (30-60 days ago)
        var previousPeriodContracts = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId &&
                       c.CreatedAt >= sixtyDaysAgo &&
                       c.CreatedAt < thirtyDaysAgo &&
                       c.Status == ContractStatus.Active)
            .CountAsync();

        // Current period revenue (last 30 days)
        var currentPeriodRevenue = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId &&
                       c.CreatedAt >= thirtyDaysAgo &&
                       c.Status != ContractStatus.Cancelled)
            .SumAsync(c => (decimal?)c.TotalAmount) ?? 0;

        // Previous period revenue (30-60 days ago)
        var previousPeriodRevenue = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId &&
                       c.CreatedAt >= sixtyDaysAgo &&
                       c.CreatedAt < thirtyDaysAgo &&
                       c.Status != ContractStatus.Cancelled)
            .SumAsync(c => (decimal?)c.TotalAmount) ?? 0;

        // Current period pending orders (last 30 days)
        var currentPeriodPendingOrders = await _dbContext.Orders
            .Where(o => o.ContractId.HasValue && userContractIds.Contains(o.ContractId.Value))
            .Where(o => o.CreatedAt >= thirtyDaysAgo &&
                       (o.Status == OrderStatus.New ||
                        o.Status == OrderStatus.Assigned ||
                        o.Status == OrderStatus.InProduction))
            .CountAsync();

        // Previous period pending orders (30-60 days ago)
        var previousPeriodPendingOrders = await _dbContext.Orders
            .Where(o => o.ContractId.HasValue && userContractIds.Contains(o.ContractId.Value))
            .Where(o => o.CreatedAt >= sixtyDaysAgo &&
                       o.CreatedAt < thirtyDaysAgo &&
                       (o.Status == OrderStatus.New ||
                        o.Status == OrderStatus.Assigned ||
                        o.Status == OrderStatus.InProduction))
            .CountAsync();

        // Calculate percentage changes
        var activeContractsChange = CalculatePercentageChange(currentPeriodContracts, previousPeriodContracts);
        var revenueChange = CalculatePercentageChange(currentPeriodRevenue, previousPeriodRevenue);
        var pendingOrdersChange = CalculatePercentageChange(currentPeriodPendingOrders, previousPeriodPendingOrders);

        return new ContractStatsDto
        {
            ActiveContracts = activeContracts,
            PendingOrders = pendingOrders,
            CompletedOrders = completedOrders,
            TotalRevenue = totalRevenue,
            RevenueChangePercentage = revenueChange,
            ActiveContractsChangePercentage = activeContractsChange,
            PendingOrdersChangePercentage = pendingOrdersChange
        };
    }

    /// <summary>
    /// Gets recent activities for the seller using EFFICIENT QUERIES
    /// NO GetAllAsync() - uses Take(), OrderBy, efficient projections
    /// </summary>
    public async Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(int limit = 10)
    {
        var userId = GetCurrentUserId();
        var activities = new List<ActivityDto>();

        // Get user's recent contracts with customer data (efficient query with limit)
        var recentContracts = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Take(limit)
            .Include(c => c.Customer)
            .Select(c => new
            {
                c.Id,
                c.ContractNumber,
                c.Status,
                CustomerName = c.Customer != null ? c.Customer.FullName : "Unknown Customer",
                UpdatedAt = c.UpdatedAt ?? c.CreatedAt
            })
            .ToListAsync();

        foreach (var contract in recentContracts)
        {
            activities.Add(new ActivityDto
            {
                Id = contract.Id,
                Type = contract.Status == ContractStatus.Active ? "ContractActive" : "ContractUpdated",
                Title = $"Contract {contract.ContractNumber}",
                Description = $"{contract.Status} - {contract.CustomerName}",
                RelatedEntityType = "Contract",
                RelatedEntityId = contract.Id,
                CreatedAt = contract.UpdatedAt,
                StatusIndicator = contract.Status.ToString()
            });
        }

        // Get contract IDs for orders query
        var userContractIds = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId)
            .Select(c => c.Id)
            .ToListAsync();

        // Get recent orders for user's contracts (efficient query with limit)
        var recentOrders = await _dbContext.Orders
            .Where(o => o.ContractId.HasValue && userContractIds.Contains(o.ContractId.Value))
            .OrderByDescending(o => o.UpdatedAt ?? o.CreatedAt)
            .Take(limit)
            .Include(o => o.FurnitureTypes)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.Status,
                FurnitureTypeName = o.FurnitureTypes != null && o.FurnitureTypes.Any()
                    ? o.FurnitureTypes.FirstOrDefault()!.Name
                    : "Furniture Item",
                UpdatedAt = o.UpdatedAt ?? o.CreatedAt
            })
            .ToListAsync();

        foreach (var order in recentOrders)
        {
            activities.Add(new ActivityDto
            {
                Id = order.Id,
                Type = "OrderUpdated",
                Title = $"Order #{order.OrderNumber}",
                Description = $"{order.Status} - {order.FurnitureTypeName}",
                RelatedEntityType = "Order",
                RelatedEntityId = order.Id,
                CreatedAt = order.UpdatedAt,
                StatusIndicator = order.Status.ToString()
            });
        }

        // Sort all activities by date and take the requested limit
        return activities
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .ToList();
    }

    /// <summary>
    /// Gets pending items for the seller using EFFICIENT QUERIES
    /// NO GetAllAsync() - uses Where, Include, efficient projections
    /// </summary>
    public async Task<IEnumerable<PendingItemDto>> GetPendingItemsAsync()
    {
        var userId = GetCurrentUserId();
        var pendingItems = new List<PendingItemDto>();
        var now = DateTime.UtcNow;

        // ===================================================================
        // 1. PENDING APPROVAL CONTRACTS (Status = New)
        // ===================================================================
        var pendingContracts = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId && c.Status == ContractStatus.New)
            .Include(c => c.Customer)
            .Select(c => new
            {
                c.Id,
                c.ContractNumber,
                c.SignedDate,
                c.DeadlineDate,
                c.CreatedAt,
                c.TotalAmount,
                CustomerName = c.Customer != null ? c.Customer.FullName : null
            })
            .ToListAsync();

        foreach (var contract in pendingContracts)
        {
            pendingItems.Add(new PendingItemDto
            {
                Id = contract.Id,
                Type = "PendingApproval",
                Title = $"Contract {contract.ContractNumber} Pending",
                Description = $"Contract awaiting approval from customer",
                Priority = "Medium",
                RelatedEntityType = "Contract",
                RelatedEntityId = contract.Id,
                DueDate = contract.DeadlineDate,
                CreatedAt = contract.CreatedAt,
                CustomerName = contract.CustomerName,
                Amount = contract.TotalAmount
            });
        }

        // ===================================================================
        // 2. PENDING ORDERS (Status = New, requires follow-up)
        // ===================================================================
        var userContractIds = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId)
            .Select(c => c.Id)
            .ToListAsync();

        var pendingOrders = await _dbContext.Orders
            .Where(o => o.ContractId.HasValue &&
                       userContractIds.Contains(o.ContractId.Value) &&
                       o.Status == OrderStatus.New)
            .Include(o => o.Contract)
                .ThenInclude(c => c!.Customer)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.DeadlineDate,
                o.CreatedAt,
                CustomerName = o.Contract != null && o.Contract.Customer != null
                    ? o.Contract.Customer.FullName
                    : null
            })
            .ToListAsync();

        foreach (var order in pendingOrders)
        {
            pendingItems.Add(new PendingItemDto
            {
                Id = order.Id,
                Type = "RequiresFollowUp",
                Title = $"Order #{order.OrderNumber} Pending",
                Description = $"Order pending production start",
                Priority = "High",
                RelatedEntityType = "Order",
                RelatedEntityId = order.Id,
                DueDate = order.DeadlineDate,
                CreatedAt = order.CreatedAt,
                CustomerName = order.CustomerName
            });
        }

        // ===================================================================
        // 3. PAYMENT ISSUES (Remaining amount > 0 and past deadline)
        // ===================================================================
        var paymentIssues = await _dbContext.Contracts
            .Where(c => c.CreatedBy == userId &&
                       c.RemainingAmount > 0 &&
                       c.DeadlineDate < now &&
                       c.Status == ContractStatus.Active)
            .Include(c => c.Customer)
            .Select(c => new
            {
                c.Id,
                c.ContractNumber,
                c.DeadlineDate,
                c.CreatedAt,
                c.RemainingAmount,
                CustomerName = c.Customer != null ? c.Customer.FullName : null
            })
            .ToListAsync();

        foreach (var contract in paymentIssues)
        {
            pendingItems.Add(new PendingItemDto
            {
                Id = contract.Id,
                Type = "AwaitingPayment",
                Title = $"Payment Due: {contract.ContractNumber}",
                Description = $"Remaining amount past deadline",
                Priority = "Urgent",
                RelatedEntityType = "Contract",
                RelatedEntityId = contract.Id,
                DueDate = contract.DeadlineDate,
                CreatedAt = contract.CreatedAt,
                CustomerName = contract.CustomerName,
                Amount = contract.RemainingAmount
            });
        }

        // Sort by priority and due date
        return pendingItems
            .OrderByDescending(p => p.Priority == "Urgent" ? 4 : p.Priority == "High" ? 3 : p.Priority == "Medium" ? 2 : 1)
            .ThenBy(p => p.DueDate)
            .ToList();
    }

    private decimal CalculatePercentageChange(decimal current, decimal previous)
    {
        if (previous == 0)
        {
            return current > 0 ? 100 : 0;
        }
        return ((current - previous) / previous) * 100;
    }

    private decimal CalculatePercentageChange(int current, int previous)
    {
        if (previous == 0)
        {
            return current > 0 ? 100 : 0;
        }
        return ((decimal)(current - previous) / previous) * 100;
    }
}
