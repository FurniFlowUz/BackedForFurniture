using AutoMapper;
using FurniFlowUz.Application.DTOs.OrderCategory;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for managing Order-Category relationships (Many-to-Many)
/// </summary>
public class OrderCategoryService : IOrderCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderCategoryService> _logger;

    public OrderCategoryService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OrderCategoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderCategoryDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        var orderCategories = await _unitOfWork.OrderCategories.FindAsync(
            oc => oc.OrderId == orderId,
            cancellationToken);

        var result = new List<OrderCategoryDto>();
        foreach (var oc in orderCategories)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(oc.CategoryId, cancellationToken);
            result.Add(new OrderCategoryDto
            {
                Id = oc.Id,
                OrderId = oc.OrderId,
                OrderNumber = order.OrderNumber,
                CategoryId = oc.CategoryId,
                CategoryName = category?.Name ?? "Unknown",
                Category = category != null ? _mapper.Map<DTOs.Category.CategoryDto>(category) : null,
                CreatedAt = oc.CreatedAt,
                DeadlineDate = order.DeadlineDate
            });
        }

        return result;
    }

    public async Task<IEnumerable<OrderCategoryDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), categoryId);
        }

        var orderCategories = await _unitOfWork.OrderCategories.FindAsync(
            oc => oc.CategoryId == categoryId,
            cancellationToken);

        var result = new List<OrderCategoryDto>();
        foreach (var oc in orderCategories)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(oc.OrderId, cancellationToken);
            result.Add(new OrderCategoryDto
            {
                Id = oc.Id,
                OrderId = oc.OrderId,
                OrderNumber = order?.OrderNumber ?? "Unknown",
                CategoryId = oc.CategoryId,
                CategoryName = category.Name,
                Category = _mapper.Map<DTOs.Category.CategoryDto>(category),
                CreatedAt = oc.CreatedAt
            });
        }

        return result;
    }

    public async Task<OrderCategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var orderCategory = await _unitOfWork.OrderCategories.GetByIdAsync(id, cancellationToken);
        if (orderCategory == null)
        {
            throw new NotFoundException(nameof(OrderCategory), id);
        }

        var order = await _unitOfWork.Orders.GetByIdAsync(orderCategory.OrderId, cancellationToken);
        var category = await _unitOfWork.Categories.GetByIdAsync(orderCategory.CategoryId, cancellationToken);

        return new OrderCategoryDto
        {
            Id = orderCategory.Id,
            OrderId = orderCategory.OrderId,
            OrderNumber = order?.OrderNumber ?? "Unknown",
            CategoryId = orderCategory.CategoryId,
            CategoryName = category?.Name ?? "Unknown",
            Category = category != null ? _mapper.Map<DTOs.Category.CategoryDto>(category) : null,
            CreatedAt = orderCategory.CreatedAt
        };
    }

    public async Task<OrderCategoryDto> AddCategoryToOrderAsync(CreateOrderCategoryDto request, CancellationToken cancellationToken = default)
    {
        // Validate Order exists
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        // Validate Category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        // Check if already exists
        var existing = await _unitOfWork.OrderCategories.FindAsync(
            oc => oc.OrderId == request.OrderId && oc.CategoryId == request.CategoryId,
            cancellationToken);

        if (existing.Any())
        {
            throw new BusinessException($"Category '{category.Name}' is already assigned to order '{order.OrderNumber}'");
        }

        var orderCategory = new OrderCategory
        {
            OrderId = request.OrderId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.OrderCategories.AddAsync(orderCategory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category {CategoryId} added to Order {OrderId}", request.CategoryId, request.OrderId);

        return new OrderCategoryDto
        {
            Id = orderCategory.Id,
            OrderId = orderCategory.OrderId,
            OrderNumber = order.OrderNumber,
            CategoryId = orderCategory.CategoryId,
            CategoryName = category.Name,
            Category = _mapper.Map<DTOs.Category.CategoryDto>(category),
            CreatedAt = orderCategory.CreatedAt
        };
    }

    public async Task RemoveCategoryFromOrderAsync(int orderId, int categoryId, CancellationToken cancellationToken = default)
    {
        var orderCategories = await _unitOfWork.OrderCategories.FindAsync(
            oc => oc.OrderId == orderId && oc.CategoryId == categoryId,
            cancellationToken);

        var orderCategory = orderCategories.FirstOrDefault();
        if (orderCategory == null)
        {
            throw new NotFoundException("OrderCategory", $"OrderId: {orderId}, CategoryId: {categoryId}");
        }

        _unitOfWork.OrderCategories.Remove(orderCategory);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category {CategoryId} removed from Order {OrderId}", categoryId, orderId);
    }

    public async Task<IEnumerable<OrderCategoryDto>> SetOrderCategoriesAsync(BulkOrderCategoryDto request, CancellationToken cancellationToken = default)
    {
        // Validate Order exists
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        // Validate all categories exist
        foreach (var categoryId in request.CategoryIds)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), categoryId);
            }
        }

        // Remove existing categories
        var existingCategories = await _unitOfWork.OrderCategories.FindAsync(
            oc => oc.OrderId == request.OrderId,
            cancellationToken);

        foreach (var existing in existingCategories)
        {
            _unitOfWork.OrderCategories.Remove(existing);
        }

        // Add new categories
        var result = new List<OrderCategoryDto>();
        foreach (var categoryId in request.CategoryIds.Distinct())
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);

            var orderCategory = new OrderCategory
            {
                OrderId = request.OrderId,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.OrderCategories.AddAsync(orderCategory, cancellationToken);

            result.Add(new OrderCategoryDto
            {
                Id = orderCategory.Id,
                OrderId = orderCategory.OrderId,
                OrderNumber = order.OrderNumber,
                CategoryId = categoryId,
                CategoryName = category?.Name ?? "Unknown",
                Category = category != null ? _mapper.Map<DTOs.Category.CategoryDto>(category) : null,
                CreatedAt = orderCategory.CreatedAt
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Set {Count} categories for Order {OrderId}", request.CategoryIds.Count, request.OrderId);

        return result;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var orderCategory = await _unitOfWork.OrderCategories.GetByIdAsync(id, cancellationToken);
        if (orderCategory == null)
        {
            throw new NotFoundException(nameof(OrderCategory), id);
        }

        _unitOfWork.OrderCategories.Remove(orderCategory);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OrderCategory {Id} deleted", id);
    }
}
