using AutoMapper;
using FurniFlowUz.Application.DTOs.Category;
using FurniFlowUz.Application.DTOs.Constructor;
using FurniFlowUz.Application.DTOs.Customer;
using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Data;
using FurniFlowUz.Infrastructure.Repositories;
using FurniFlowUz.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for constructor operations (furniture design and technical specifications)
/// </summary>
public class ConstructorService : IConstructorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public ConstructorService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByConstructorAsync(int constructorId, CancellationToken cancellationToken = default)
    {
        // Validate constructor exists and has correct role
        var constructor = await _unitOfWork.Users.GetByIdAsync(constructorId, cancellationToken);
        if (constructor == null)
        {
            throw new NotFoundException(nameof(User), constructorId);
        }

        if (constructor.Role != UserRole.Constructor)
        {
            throw new ValidationException("User must have Constructor role.");
        }

        // Get all orders assigned to this constructor
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var constructorOrders = allOrders
            .Where(o => o.AssignedConstructorId == constructorId)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        // Get related data
        var orderIds = constructorOrders.Select(o => o.Id).ToList();

        // Get all furniture types for these orders
        var allFurnitureTypes = await _unitOfWork.FurnitureTypes.GetAllAsync(cancellationToken);
        var furnitureTypesByOrder = allFurnitureTypes
            .Where(ft => orderIds.Contains(ft.OrderId))
            .GroupBy(ft => ft.OrderId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Get all categories
        var allCategories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        var categoriesDict = allCategories.ToDictionary(c => c.Id);

        // Get all customers
        var customerIds = constructorOrders.Select(o => o.CustomerId).Distinct().ToList();
        var allCustomers = await _unitOfWork.Customers.GetAllAsync(cancellationToken);
        var customersDict = allCustomers
            .Where(c => customerIds.Contains(c.Id))
            .ToDictionary(c => c.Id);

        // Get all order images count
        var allOrderImages = await _unitOfWork.OrderImages.GetAllAsync(cancellationToken);
        var imagesCountByOrder = allOrderImages
            .Where(oi => orderIds.Contains(oi.OrderId) && !oi.IsDeleted)
            .GroupBy(oi => oi.OrderId)
            .ToDictionary(g => g.Key, g => g.Count());

        // Map to DTOs
        var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(constructorOrders).ToList();

        // Populate related data for each order
        foreach (var orderDto in orderDtos)
        {
            if (orderDto.Id.HasValue)
            {
                // Add furniture types
                if (furnitureTypesByOrder.ContainsKey(orderDto.Id.Value))
                {
                    orderDto.FurnitureTypes = _mapper.Map<List<FurnitureTypeDto>>(
                        furnitureTypesByOrder[orderDto.Id.Value]
                    );
                }
            }

            // Add category
            var originalOrder = constructorOrders.First(o => o.Id == orderDto.Id);
            if (categoriesDict.ContainsKey(originalOrder.CategoryId))
            {
                orderDto.Category = _mapper.Map<CategoryDto>(categoriesDict[originalOrder.CategoryId]);
            }

            // Add customer
            if (customersDict.ContainsKey(originalOrder.CustomerId))
            {
                orderDto.Customer = _mapper.Map<CustomerDto>(customersDict[originalOrder.CustomerId]);
            }

            // Add images count
            if (orderDto.Id.HasValue && imagesCountByOrder.ContainsKey(orderDto.Id.Value))
            {
                orderDto.ImagesCount = imagesCountByOrder[orderDto.Id.Value];
            }
        }

        return orderDtos;
    }

    /* public async Task<IEnumerable<OrderDto>> GetOrdersByConstructorAsync(int constructorId, CancellationToken cancellationToken = default)
     {
         // Validate constructor exists and has correct role
         var constructor = await _unitOfWork.Users.GetByIdAsync(constructorId, cancellationToken);
         if (constructor == null)
         {
             throw new NotFoundException(nameof(User), constructorId);
         }

         if (constructor.Role != UserRole.Constructor)
         {
             throw new ValidationException("User must have Constructor role.");
         }

         // Get all orders assigned to this constructor
         var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
         var constructorOrders = allOrders
             .Where(o => o.AssignedConstructorId == constructorId)

             .OrderByDescending(o => o.CreatedAt)
             .ToList();

         return _mapper.Map<IEnumerable<OrderDto>>(constructorOrders);

     }
 */
    public async Task<IEnumerable<FurnitureTypeDto>> GetFurnitureTypesByOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        // Validate order exists
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        // ✅ Verify user has access to this order
        if (_currentUserService.Role == "Constructor")
        {
            if (order.AssignedConstructorId != _currentUserService.UserId)
            {
                throw new UnauthorizedAccessException("You do not have access to this order.");
            }
        }

        // Get all furniture types for this order
        var allFurnitureTypes = await _unitOfWork.FurnitureTypes.GetAllAsync(cancellationToken);
        var orderFurnitureTypes = allFurnitureTypes
            .Where(ft => ft.OrderId == orderId && !ft.IsDeleted)
            .OrderBy(ft => ft.CreatedAt)
            .ToList();

        return _mapper.Map<IEnumerable<FurnitureTypeDto>>(orderFurnitureTypes);
    }

    public async Task<FurnitureTypeDto> GetFurnitureTypeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(id, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), id);
        }

        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(id, cancellationToken);

        return _mapper.Map<FurnitureTypeDto>(furnitureType);
    }

    public async Task<FurnitureTypeDto> CreateFurnitureTypeAsync(CreateFurnitureTypeDto request, CancellationToken cancellationToken = default)
    {
        // Validate order exists
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var furnitureType = new FurnitureType
        {
            Name = request.Name,
            OrderId = request.OrderId,
            ProgressPercentage = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FurnitureTypes.AddAsync(furnitureType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FurnitureTypeDto>(furnitureType);
    }

    public async Task<FurnitureTypeDto> UpdateFurnitureTypeAsync(int id, UpdateFurnitureTypeDto request, CancellationToken cancellationToken = default)
    {
        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(id, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), id);
        }

        // Check if technical specification is locked
        if (furnitureType.TechnicalSpecificationId.HasValue)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec?.IsLocked == true)
            {
                throw new BusinessException("Cannot edit furniture type with locked technical specification.");
            }
        }

        furnitureType.Name = request.Name;
        furnitureType.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.FurnitureTypes.Update(furnitureType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FurnitureTypeDto>(furnitureType);
    }

    public async Task DeleteFurnitureTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(id, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), id);
        }

        // Check if technical specification is locked
        if (furnitureType.TechnicalSpecificationId.HasValue)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec?.IsLocked == true)
            {
                throw new BusinessException("Cannot delete furniture type with locked technical specification.");
            }
        }

        // Delete associated details and drawings
        var details = await _unitOfWork.Details.FindAsync(d => d.FurnitureTypeId == id, cancellationToken);
        _unitOfWork.Details.RemoveRange(details);

        var drawings = await _unitOfWork.Drawings.FindAsync(d => d.FurnitureTypeId == id, cancellationToken);
        foreach (var drawing in drawings)
        {
            await _fileStorageService.DeleteFileAsync(drawing.FileName, "drawings");
        }
        _unitOfWork.Drawings.RemoveRange(drawings);

        _unitOfWork.FurnitureTypes.Remove(furnitureType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ForceDeleteFurnitureTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(id, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), id);
        }

        // Delete associated technical specification if exists (even if locked)
        if (furnitureType.TechnicalSpecificationId.HasValue)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec != null)
            {
                _unitOfWork.TechnicalSpecifications.Remove(techSpec);
            }
        }

        // Delete associated details
        var details = await _unitOfWork.Details.FindAsync(d => d.FurnitureTypeId == id, cancellationToken);
        _unitOfWork.Details.RemoveRange(details);

        // Delete associated drawings and their files
        var drawings = await _unitOfWork.Drawings.FindAsync(d => d.FurnitureTypeId == id, cancellationToken);
        foreach (var drawing in drawings)
        {
            await _fileStorageService.DeleteFileAsync(drawing.FileName, "drawings");
        }
        _unitOfWork.Drawings.RemoveRange(drawings);

        // Delete the furniture type
        _unitOfWork.FurnitureTypes.Remove(furnitureType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<DetailDto>> GetAllConstructorDetailsAsync(int constructorId, CancellationToken cancellationToken = default)
    {
        // Get all orders assigned to the constructor
        var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var constructorOrders = orders.Where(o => o.AssignedConstructorId == constructorId && !o.IsDeleted).ToList();

        if (!constructorOrders.Any())
        {
            return Enumerable.Empty<DetailDto>();
        }

        // Get all furniture types for these orders
        var furnitureTypes = await _unitOfWork.FurnitureTypes.GetAllAsync(cancellationToken);
        var constructorFurnitureTypeIds = furnitureTypes
            .Where(ft => constructorOrders.Any(o => o.Id == ft.OrderId) && !ft.IsDeleted)
            .Select(ft => ft.Id)
            .ToList();

        if (!constructorFurnitureTypeIds.Any())
        {
            return Enumerable.Empty<DetailDto>();
        }

        // Get all details for these furniture types
        var allDetails = await _unitOfWork.Details.GetAllAsync(cancellationToken);
        var constructorDetails = allDetails
            .Where(d => constructorFurnitureTypeIds.Contains(d.FurnitureTypeId) && !d.IsDeleted)
            .OrderBy(d => d.FurnitureTypeId)
            .ThenBy(d => d.Name)
            .ToList();

        return _mapper.Map<IEnumerable<DetailDto>>(constructorDetails);
    }

    public async Task<IEnumerable<DetailDto>> GetDetailsByFurnitureTypeAsync(int furnitureTypeId, CancellationToken cancellationToken = default)
    {
        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(furnitureTypeId, cancellationToken);

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(furnitureTypeId, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), furnitureTypeId);
        }

        var details = await _unitOfWork.Details.GetAllAsync(cancellationToken);
        var furnitureTypeDetails = details.Where(d => d.FurnitureTypeId == furnitureTypeId && !d.IsDeleted);

        return _mapper.Map<IEnumerable<DetailDto>>(furnitureTypeDetails);
    }

    public async Task<DetailDto> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var detail = await _unitOfWork.Details.GetByIdAsync(id, cancellationToken);
        if (detail == null)
        {
            throw new NotFoundException(nameof(Detail), id);
        }

        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(detail.FurnitureTypeId, cancellationToken);

        return _mapper.Map<DetailDto>(detail);
    }

    public async Task<DetailDto> CreateDetailAsync(CreateDetailDto request, CancellationToken cancellationToken = default)
    {
        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(request.FurnitureTypeId, cancellationToken);

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(request.FurnitureTypeId, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), request.FurnitureTypeId);
        }

        // Check if technical specification is locked
        if (furnitureType.TechnicalSpecificationId.HasValue)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec?.IsLocked == true)
            {
                throw new BusinessException("Cannot add details to furniture type with locked technical specification.");
            }
        }

        var detail = new Detail
        {
            Name = request.Name,
            Width = request.Width,
            Height = request.Height,
            Thickness = request.Thickness,
            Quantity = request.Quantity,
            FurnitureTypeId = request.FurnitureTypeId,
            Material = request.Material,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Details.AddAsync(detail, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DetailDto>(detail);
    }

    public async Task<DetailDto> UpdateDetailAsync(int id, UpdateDetailDto request, CancellationToken cancellationToken = default)
    {
        var detail = await _unitOfWork.Details.GetByIdAsync(id, cancellationToken);
        if (detail == null)
        {
            throw new NotFoundException(nameof(Detail), id);
        }

        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(detail.FurnitureTypeId, cancellationToken);

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(detail.FurnitureTypeId, cancellationToken);

        // Check if technical specification is locked
        if (furnitureType?.TechnicalSpecificationId.HasValue == true)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec?.IsLocked == true)
            {
                throw new BusinessException("Cannot edit details of furniture type with locked technical specification.");
            }
        }

        detail.Name = request.Name;
        detail.Width = request.Width;
        detail.Height = request.Height;
        detail.Thickness = request.Thickness;
        detail.Quantity = request.Quantity;
        detail.Material = request.Material;
        detail.Notes = request.Notes;
        detail.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Details.Update(detail);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DetailDto>(detail);
    }

    public async Task DeleteDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        var detail = await _unitOfWork.Details.GetByIdAsync(id, cancellationToken);
        if (detail == null)
        {
            throw new NotFoundException(nameof(Detail), id);
        }

        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(detail.FurnitureTypeId, cancellationToken);

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(detail.FurnitureTypeId, cancellationToken);

        // Check if technical specification is locked
        if (furnitureType?.TechnicalSpecificationId.HasValue == true)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec?.IsLocked == true)
            {
                throw new BusinessException("Cannot delete details of furniture type with locked technical specification.");
            }
        }

        _unitOfWork.Details.Remove(detail);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<DrawingDto>> GetAllConstructorDrawingsAsync(int constructorId, CancellationToken cancellationToken = default)
    {
        // Get all orders assigned to the constructor
        var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var constructorOrders = orders.Where(o => o.AssignedConstructorId == constructorId && !o.IsDeleted).ToList();

        if (!constructorOrders.Any())
        {
            return Enumerable.Empty<DrawingDto>();
        }

        // Get all furniture types for these orders
        var furnitureTypes = await _unitOfWork.FurnitureTypes.GetAllAsync(cancellationToken);
        var constructorFurnitureTypeIds = furnitureTypes
            .Where(ft => constructorOrders.Any(o => o.Id == ft.OrderId) && !ft.IsDeleted)
            .Select(ft => ft.Id)
            .ToList();

        if (!constructorFurnitureTypeIds.Any())
        {
            return Enumerable.Empty<DrawingDto>();
        }

        // Get all drawings for these furniture types
        var allDrawings = await _unitOfWork.Drawings.GetAllAsync(cancellationToken);
        var constructorDrawings = allDrawings
            .Where(d => constructorFurnitureTypeIds.Contains(d.FurnitureTypeId) && !d.IsDeleted)
            .OrderBy(d => d.FurnitureTypeId)
            .ThenByDescending(d => d.UploadedAt)
            .ToList();

        return _mapper.Map<IEnumerable<DrawingDto>>(constructorDrawings);
    }

    public async Task<DrawingDto> UploadDrawingAsync(int furnitureTypeId, IFormFile file, string? description, CancellationToken cancellationToken = default)
    {
        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(furnitureTypeId, cancellationToken);

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(furnitureTypeId, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), furnitureTypeId);
        }

        // Check if technical specification is locked
        if (furnitureType.TechnicalSpecificationId.HasValue)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec?.IsLocked == true)
            {
                throw new BusinessException("Cannot upload drawings for furniture type with locked technical specification.");
            }
        }

        // Upload file using file storage service
        var fileName = await _fileStorageService.UploadFileAsync(file, "drawings");
        var filePath = _fileStorageService.GetFileUrl(fileName, "drawings");

        var drawing = new Drawing
        {
            FileName = fileName,
            FilePath = filePath,
            FurnitureTypeId = furnitureTypeId,
            UploadedAt = DateTime.UtcNow,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Drawings.AddAsync(drawing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DrawingDto>(drawing);
    }

    public async Task DeleteDrawingAsync(int id, CancellationToken cancellationToken = default)
    {
        var drawing = await _unitOfWork.Drawings.GetByIdAsync(id, cancellationToken);
        if (drawing == null)
        {
            throw new NotFoundException(nameof(Drawing), id);
        }

        // ✅ Verify user has access to this furniture type
        await VerifyFurnitureTypeAccessAsync(drawing.FurnitureTypeId, cancellationToken);

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(drawing.FurnitureTypeId, cancellationToken);

        // Check if technical specification is locked
        if (furnitureType?.TechnicalSpecificationId.HasValue == true)
        {
            var techSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (techSpec?.IsLocked == true)
            {
                throw new BusinessException("Cannot delete drawings of furniture type with locked technical specification.");
            }
        }

        // Delete physical file
        await _fileStorageService.DeleteFileAsync(drawing.FileName, "drawings");

        _unitOfWork.Drawings.Remove(drawing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<TechnicalSpecificationDto> CreateTechnicalSpecificationAsync(CreateTechnicalSpecificationDto request, CancellationToken cancellationToken = default)
    {
        if (!request.FurnitureTypeId.HasValue)
        {
            throw new BusinessException("Furniture type ID is required.");
        }

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(request.FurnitureTypeId.Value, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), request.FurnitureTypeId.Value);
        }

        // Check if technical specification already exists
        if (furnitureType.TechnicalSpecificationId.HasValue)
        {
            throw new BusinessException("Technical specification already exists for this furniture type.");
        }

        var technicalSpec = new TechnicalSpecification
        {
            FurnitureTypeId = request.FurnitureTypeId.Value,
            Notes = request.Notes,
            IsLocked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TechnicalSpecifications.AddAsync(technicalSpec, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Link technical specification to furniture type
        furnitureType.TechnicalSpecificationId = technicalSpec.Id;
        _unitOfWork.FurnitureTypes.Update(furnitureType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TechnicalSpecificationDto>(technicalSpec);
    }

    public async Task CompleteTechnicalSpecificationAsync(int furnitureTypeId, CancellationToken cancellationToken = default)
    {
        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(furnitureTypeId, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), furnitureTypeId);
        }

        // If no technical specification exists, create one automatically
        if (!furnitureType.TechnicalSpecificationId.HasValue)
        {
            var newTechnicalSpec = new TechnicalSpecification
            {
                FurnitureTypeId = furnitureTypeId,
                Notes = "Automatically created on completion",
                IsLocked = true,
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TechnicalSpecifications.AddAsync(newTechnicalSpec, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            furnitureType.TechnicalSpecificationId = newTechnicalSpec.Id;
            _unitOfWork.FurnitureTypes.Update(furnitureType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var technicalSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (technicalSpec == null)
            {
                throw new NotFoundException(nameof(TechnicalSpecification), furnitureType.TechnicalSpecificationId.Value);
            }

            if (technicalSpec.IsLocked)
            {
                throw new BusinessException("Technical specification is already locked.");
            }

            // Lock the specification
            technicalSpec.IsLocked = true;
            technicalSpec.CompletedAt = DateTime.UtcNow;
            technicalSpec.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TechnicalSpecifications.Update(technicalSpec);
        }

        // Check if all furniture types in the order have locked specifications
        var order = await _unitOfWork.Orders.GetByIdAsync(furnitureType.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), furnitureType.OrderId);
        }

        var allFurnitureTypes = await _unitOfWork.FurnitureTypes.FindAsync(
            f => f.OrderId == order.Id,
            cancellationToken);

        var allSpecsLocked = true;
        foreach (var ft in allFurnitureTypes)
        {
            if (!ft.TechnicalSpecificationId.HasValue)
            {
                allSpecsLocked = false;
                break;
            }

            var spec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                ft.TechnicalSpecificationId.Value,
                cancellationToken);

            if (spec?.IsLocked != true)
            {
                allSpecsLocked = false;
                break;
            }
        }

        // If all specs are locked, update order status to SpecificationsReady
        if (allSpecsLocked && (order.Status == OrderStatus.New || order.Status == OrderStatus.Assigned))
        {
            order.Status = OrderStatus.SpecificationsReady;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);

            // Send notification to production manager if assigned
            if (order.AssignedProductionManagerId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    Title = "Technical Specifications Completed",
                    Message = $"All technical specifications for order {order.OrderNumber} are completed and ready for production.",
                    Type = NotificationType.OrderStatusChanged.ToString(),
                    UserId = order.AssignedProductionManagerId.Value,
                    RelatedEntityType = "Order",
                    RelatedEntityId = order.Id
                }, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteFurnitureTypeWithDataAsync(int furnitureTypeId, CompleteFurnitureTypeDto request, CancellationToken cancellationToken = default)
    {
        // Verify furniture type exists and user has access
        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(furnitureTypeId, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), furnitureTypeId);
        }

        await VerifyFurnitureTypeAccessAsync(furnitureTypeId, cancellationToken);

        // Check if already locked
        if (furnitureType.TechnicalSpecificationId.HasValue)
        {
            var existingSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (existingSpec?.IsLocked == true)
            {
                throw new BusinessException("Technical specification is already locked.");
            }
        }

        // 1. Save all details if provided
        if (request.Details != null && request.Details.Any())
        {
            foreach (var detailItem in request.Details)
            {
                var detail = new Detail
                {
                    Name = detailItem.Name,
                    Width = detailItem.Width,
                    Height = detailItem.Height,
                    Thickness = detailItem.Thickness,
                    Quantity = detailItem.Quantity,
                    FurnitureTypeId = furnitureTypeId,
                    Material = detailItem.Material,
                    Notes = detailItem.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Details.AddAsync(detail, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // 2. Create or update technical specification
        if (!furnitureType.TechnicalSpecificationId.HasValue)
        {
            var technicalSpec = new TechnicalSpecification
            {
                FurnitureTypeId = furnitureTypeId,
                Notes = request.Notes ?? "Automatically created on completion",
                IsLocked = true,
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TechnicalSpecifications.AddAsync(technicalSpec, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            furnitureType.TechnicalSpecificationId = technicalSpec.Id;
            _unitOfWork.FurnitureTypes.Update(furnitureType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var technicalSpec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                furnitureType.TechnicalSpecificationId.Value,
                cancellationToken);

            if (technicalSpec != null)
            {
                technicalSpec.Notes = request.Notes ?? technicalSpec.Notes;
                technicalSpec.IsLocked = true;
                technicalSpec.CompletedAt = DateTime.UtcNow;
                technicalSpec.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.TechnicalSpecifications.Update(technicalSpec);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        // 3. Check if all furniture types in the order have locked specifications
        var order = await _unitOfWork.Orders.GetByIdAsync(furnitureType.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), furnitureType.OrderId);
        }

        var allFurnitureTypes = await _unitOfWork.FurnitureTypes.FindAsync(
            f => f.OrderId == order.Id,
            cancellationToken);

        var allSpecsLocked = true;
        foreach (var ft in allFurnitureTypes)
        {
            if (!ft.TechnicalSpecificationId.HasValue)
            {
                allSpecsLocked = false;
                break;
            }

            var spec = await _unitOfWork.TechnicalSpecifications.GetByIdAsync(
                ft.TechnicalSpecificationId.Value,
                cancellationToken);

            if (spec?.IsLocked != true)
            {
                allSpecsLocked = false;
                break;
            }
        }

        // If all specs are locked, update order status to SpecificationsReady
        if (allSpecsLocked && (order.Status == OrderStatus.New || order.Status == OrderStatus.Assigned))
        {
            order.Status = OrderStatus.SpecificationsReady;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);

            // Send notification to production manager if assigned
            if (order.AssignedProductionManagerId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    Title = "Technical Specifications Completed",
                    Message = $"All technical specifications for order {order.OrderNumber} are completed and ready for production.",
                    Type = NotificationType.OrderStatusChanged.ToString(),
                    UserId = order.AssignedProductionManagerId.Value,
                    RelatedEntityType = "Order",
                    RelatedEntityId = order.Id
                }, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    #region Private Helper Methods

    /// <summary>
    /// Verifies that the current user has access to the specified furniture type
    /// based on their role and the order assignment
    /// </summary>
    private async Task VerifyFurnitureTypeAccessAsync(int furnitureTypeId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var furnitureType = await _unitOfWork.FurnitureTypes.GetByIdAsync(furnitureTypeId, cancellationToken);
        if (furnitureType == null)
        {
            throw new NotFoundException(nameof(FurnitureType), furnitureTypeId);
        }

        var order = await _unitOfWork.Orders.GetByIdAsync(furnitureType.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), furnitureType.OrderId);
        }

        var currentUserId = _currentUserService.UserId.Value;
        var currentRole = _currentUserService.Role;

        if (!Enum.TryParse<UserRole>(currentRole, out var userRole))
        {
            throw new UnauthorizedAccessException("Invalid user role.");
        }

        // Check role-based access
        var hasAccess = userRole switch
        {
            UserRole.Director => true,
            UserRole.Constructor => order.AssignedConstructorId == currentUserId,
            UserRole.ProductionManager => order.AssignedProductionManagerId == currentUserId,
            UserRole.Salesperson => order.CreatedBy == currentUserId,
            _ => false
        };

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException($"You do not have permission to access furniture type {furnitureTypeId}.");
        }
    }

    /// <summary>
    /// Verifies that the current user has access to the specified order
    /// </summary>
    private async Task VerifyOrderAccessAsync(int orderId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        var currentUserId = _currentUserService.UserId.Value;
        var currentRole = _currentUserService.Role;

        if (!Enum.TryParse<UserRole>(currentRole, out var userRole))
        {
            throw new UnauthorizedAccessException("Invalid user role.");
        }

        // Check if user is a Salesperson who has access through the contract
        bool isSalespersonWithContractAccess = false;
        if (userRole == UserRole.Salesperson && order.ContractId.HasValue)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(order.ContractId.Value, cancellationToken);
            if (contract != null && contract.CreatedBy == currentUserId)
            {
                isSalespersonWithContractAccess = true;
            }
        }

        var hasAccess = userRole switch
        {
            UserRole.Director => true,
            UserRole.Constructor => order.AssignedConstructorId == currentUserId,
            UserRole.ProductionManager => order.AssignedProductionManagerId == currentUserId,
            UserRole.Salesperson => order.CreatedBy == currentUserId || isSalespersonWithContractAccess,
            _ => false
        };

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException($"You do not have permission to access order {orderId}.");
        }
    }

    #endregion

    #region Order Images

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedImageContentTypes = { "image/jpeg", "image/png", "image/webp" };
    private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB

    public async Task<OrderImageDto> UploadOrderImageAsync(UploadOrderImageRequest request, CancellationToken cancellationToken = default)
    {
        // Verify order exists and user has access
        await VerifyOrderAccessAsync(request.OrderId, cancellationToken);

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        // Validate image type
        if (request.ImageType != "room" && request.ImageType != "design")
        {
            throw new ValidationException("Image type must be 'room' or 'design'.");
        }

        // Validate file
        if (request.File == null || request.File.Length == 0)
        {
            throw new ValidationException("File is required.");
        }

        if (request.File.Length > MaxImageSizeBytes)
        {
            throw new ValidationException($"File size exceeds maximum allowed size of {MaxImageSizeBytes / (1024 * 1024)}MB.");
        }

        var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(fileExtension))
        {
            throw new ValidationException($"File type not allowed. Allowed types: {string.Join(", ", AllowedImageExtensions)}");
        }

        if (!AllowedImageContentTypes.Contains(request.File.ContentType.ToLowerInvariant()))
        {
            throw new ValidationException($"Content type not allowed. Allowed types: {string.Join(", ", AllowedImageContentTypes)}");
        }

        // Generate unique file name
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.File.FileName)}";

        // Create directory path: wwwroot/uploads/orders/{orderId}/{imageType}/
        var relativePath = Path.Combine("uploads", "orders", request.OrderId.ToString(), request.ImageType);
        var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        // Ensure directory exists
        Directory.CreateDirectory(absolutePath);

        // Full file path
        var fullFilePath = Path.Combine(absolutePath, uniqueFileName);
        var relativeFilePath = Path.Combine(relativePath, uniqueFileName).Replace("\\", "/");

        // Save file
        using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        // Create database record
        var orderImage = new OrderImage
        {
            OrderId = request.OrderId,
            FileName = request.File.FileName,
            FilePath = fullFilePath,
            FileSize = request.File.Length,
            ContentType = request.File.ContentType,
            ImageType = request.ImageType,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.OrderImages.AddAsync(orderImage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OrderImageDto
        {
            Id = orderImage.Id,
            FileUrl = $"/{relativeFilePath}",
            ImageType = orderImage.ImageType,
            FileName = orderImage.FileName,
            FileSize = orderImage.FileSize,
            UploadedAt = orderImage.UploadedAt
        };
    }

    public async Task<IEnumerable<OrderImageDto>> GetOrderImagesAsync(int orderId, CancellationToken cancellationToken = default)
    {
        // Verify order exists and user has access
        await VerifyOrderAccessAsync(orderId, cancellationToken);

        var images = await _unitOfWork.OrderImages.FindAsync(
            oi => oi.OrderId == orderId && !oi.IsDeleted,
            cancellationToken);

        return images.Select(img =>
        {
            // Convert absolute path to relative URL
            var relativePath = Path.Combine("uploads", "orders", img.OrderId.ToString(), img.ImageType, Path.GetFileName(img.FilePath));

            return new OrderImageDto
            {
                Id = img.Id,
                FileUrl = $"/{relativePath.Replace("\\", "/")}",
                ImageType = img.ImageType,
                FileName = img.FileName,
                FileSize = img.FileSize,
                UploadedAt = img.UploadedAt
            };
        }).ToList();
    }

    public async Task DeleteOrderImageAsync(int imageId, CancellationToken cancellationToken = default)
    {
        var image = await _unitOfWork.OrderImages.GetByIdAsync(imageId, cancellationToken);
        if (image == null)
        {
            throw new NotFoundException(nameof(OrderImage), imageId);
        }

        // Verify user has access to the order
        await VerifyOrderAccessAsync(image.OrderId, cancellationToken);

        // Delete physical file
        if (File.Exists(image.FilePath))
        {
            File.Delete(image.FilePath);
        }

        // Delete database record
        _unitOfWork.OrderImages.Remove(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
