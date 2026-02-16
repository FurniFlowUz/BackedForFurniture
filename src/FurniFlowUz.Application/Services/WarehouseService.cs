using AutoMapper;
using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.DTOs.Warehouse;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for warehouse and inventory management
/// </summary>
public class WarehouseService : IWarehouseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public WarehouseService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<WarehouseItemDto>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.WarehouseItems.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseItemDto>>(items);
    }

    public async Task<WarehouseItemDto> GetItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.WarehouseItems.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), id);
        }

        return _mapper.Map<WarehouseItemDto>(item);
    }

    public async Task<WarehouseItemDto> CreateItemAsync(CreateWarehouseItemDto request, CancellationToken cancellationToken = default)
    {
        // Check if SKU already exists
        var existingItems = await _unitOfWork.WarehouseItems.FindAsync(
            i => i.SKU.ToLower() == request.SKU.ToLower(),
            cancellationToken);

        if (existingItems.Any())
        {
            throw new ValidationException($"Warehouse item with SKU '{request.SKU}' already exists.");
        }

        // Parse unit from string to enum
        if (!Enum.TryParse<UnitOfMeasurement>(request.Unit, true, out var unit))
        {
            throw new ValidationException($"Invalid unit: {request.Unit}");
        }

        var item = new WarehouseItem
        {
            Name = request.Name,
            SKU = request.SKU,
            CurrentStock = request.CurrentStock,
            MinimumStock = request.MinimumStock,
            Unit = unit,
            UnitPrice = request.UnitPrice,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.WarehouseItems.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WarehouseItemDto>(item);
    }

    public async Task<WarehouseItemDto> UpdateItemAsync(int id, UpdateWarehouseItemDto request, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.WarehouseItems.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), id);
        }

        // Check if new SKU conflicts with existing items
        if (item.SKU != request.SKU)
        {
            var existingItems = await _unitOfWork.WarehouseItems.FindAsync(
                i => i.SKU.ToLower() == request.SKU.ToLower() && i.Id != id,
                cancellationToken);

            if (existingItems.Any())
            {
                throw new ValidationException($"Warehouse item with SKU '{request.SKU}' already exists.");
            }
        }

        // Parse unit from string to enum
        if (!Enum.TryParse<UnitOfMeasurement>(request.Unit, true, out var unit))
        {
            throw new ValidationException($"Invalid unit: {request.Unit}");
        }

        item.Name = request.Name;
        item.SKU = request.SKU;
        item.MinimumStock = request.MinimumStock;
        item.Unit = unit;
        item.UnitPrice = request.UnitPrice;
        item.Description = request.Description;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WarehouseItems.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WarehouseItemDto>(item);
    }

    public async Task DeleteItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.WarehouseItems.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), id);
        }

        // Check if item has transactions
        var transactions = await _unitOfWork.WarehouseTransactions.FindAsync(
            t => t.WarehouseItemId == id,
            cancellationToken);

        if (transactions.Any())
        {
            throw new BusinessException("Cannot delete warehouse item with transaction history.");
        }

        _unitOfWork.WarehouseItems.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<WarehouseTransactionDto> CreateIncomeTransactionAsync(CreateIncomeTransactionDto request, CancellationToken cancellationToken = default)
    {
        // Validate warehouse item exists
        var item = await _unitOfWork.WarehouseItems.GetByIdAsync(request.WarehouseItemId, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), request.WarehouseItemId);
        }

        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.CreatedByUserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.CreatedByUserId);
        }

        if (user.Role != UserRole.WarehouseManager)
        {
            throw new ValidationException("Only warehouse managers can create income transactions.");
        }

        // Create income transaction
        var transaction = new WarehouseTransaction
        {
            Type = WarehouseTransactionType.Income,
            WarehouseItemId = request.WarehouseItemId,
            Quantity = request.Quantity,
            CreatedByUserId = request.CreatedByUserId,
            TransactionDate = DateTime.UtcNow,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.WarehouseTransactions.AddAsync(transaction, cancellationToken);

        // Update warehouse item stock
        item.CurrentStock += request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.WarehouseItems.Update(item);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WarehouseTransactionDto>(transaction);
    }

    public async Task<WarehouseTransactionDto> CreateOutcomeTransactionAsync(CreateOutcomeTransactionDto request, CancellationToken cancellationToken = default)
    {
        // Validate warehouse item exists
        var item = await _unitOfWork.WarehouseItems.GetByIdAsync(request.WarehouseItemId, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), request.WarehouseItemId);
        }

        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.CreatedByUserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.CreatedByUserId);
        }

        if (user.Role != UserRole.WarehouseManager)
        {
            throw new ValidationException("Only warehouse managers can create outcome transactions.");
        }

        // Validate team exists
        var team = await _unitOfWork.Teams.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
        {
            throw new NotFoundException(nameof(Team), request.TeamId);
        }

        // Check if sufficient stock is available
        if (item.CurrentStock < request.Quantity)
        {
            throw new BusinessException($"Insufficient stock. Available: {item.CurrentStock}, Requested: {request.Quantity}");
        }

        // Create outcome transaction
        var transaction = new WarehouseTransaction
        {
            Type = WarehouseTransactionType.Outcome,
            WarehouseItemId = request.WarehouseItemId,
            Quantity = request.Quantity,
            TeamId = request.TeamId,
            CreatedByUserId = request.CreatedByUserId,
            TransactionDate = DateTime.UtcNow,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.WarehouseTransactions.AddAsync(transaction, cancellationToken);

        // Update warehouse item stock
        item.CurrentStock -= request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.WarehouseItems.Update(item);

        // Create material request for team confirmation
        var materialRequest = new MaterialRequest
        {
            WarehouseTransactionId = transaction.Id,
            TeamId = request.TeamId,
            RequestedByUserId = request.CreatedByUserId,
            ConfirmationStatus = ConfirmationStatus.Pending,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.MaterialRequests.AddAsync(materialRequest, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to team leader for material confirmation
        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            Title = "Material Qabul Qilishni Tasdiqlang",
            Message = $"{item.Name} dan {request.Quantity} {item.Unit} qabul qilganingizni tasdiqlang.",
            Type = NotificationType.MaterialConfirmationPending.ToString(),
            UserId = team.TeamLeaderId
        }, cancellationToken);

        // Check for low stock and send alerts
        if (item.CurrentStock < item.MinimumStock)
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Kam Zaxira Ogohlantirish",
                Message = $"'{item.Name}' ombor mahsuloti minimal zaxira darajasidan past. Joriy: {item.CurrentStock}, Minimal: {item.MinimumStock}",
                Type = NotificationType.MaterialShortage.ToString(),
                Role = UserRole.Director.ToString()
            }, cancellationToken);

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Kam Zaxira Ogohlantirish",
                Message = $"'{item.Name}' ombor mahsuloti minimal zaxira darajasidan past. Joriy: {item.CurrentStock}, Minimal: {item.MinimumStock}",
                Type = NotificationType.MaterialShortage.ToString(),
                Role = UserRole.WarehouseManager.ToString()
            }, cancellationToken);
        }

        return _mapper.Map<WarehouseTransactionDto>(transaction);
    }

    public async Task<IEnumerable<WarehouseAlertDto>> GetLowStockAlertsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.WarehouseItems.GetAllAsync(cancellationToken);

        var lowStockItems = items.Where(i => i.IsActive && i.CurrentStock < i.MinimumStock);

        var alerts = lowStockItems.Select(item => new WarehouseAlertDto
        {
            WarehouseItemId = item.Id,
            ItemName = item.Name,
            SKU = item.SKU,
            CurrentStock = item.CurrentStock,
            MinimumStock = item.MinimumStock,
            Unit = item.Unit.ToString(),
            Shortage = item.MinimumStock - item.CurrentStock,
            AlertMessage = $"{item.Name} minimal zaxira darajasidan past"
        });

        return alerts;
    }

    public async Task<IEnumerable<WarehouseTransactionDto>> GetStockHistoryAsync(int itemId, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.WarehouseItems.GetByIdAsync(itemId, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException(nameof(WarehouseItem), itemId);
        }

        var transactions = await _unitOfWork.WarehouseTransactions.FindAsync(
            t => t.WarehouseItemId == itemId,
            cancellationToken);

        return _mapper.Map<IEnumerable<WarehouseTransactionDto>>(
            transactions.OrderByDescending(t => t.TransactionDate));
    }
}
