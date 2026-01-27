using FurniFlowUz.Application.DTOs.Constructor;
using FurniFlowUz.Application.DTOs.Order;
using Microsoft.AspNetCore.Http;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for constructor operations (furniture design and technical specifications)
/// </summary>
public interface IConstructorService
{
    /// <summary>
    /// Gets all orders assigned to a specific constructor
    /// </summary>
    Task<IEnumerable<OrderDto>> GetOrdersByConstructorAsync(int constructorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all furniture types for a specific order
    /// </summary>
    Task<IEnumerable<FurnitureTypeDto>> GetFurnitureTypesByOrderAsync(int orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific furniture type by ID
    /// </summary>
    Task<FurnitureTypeDto> GetFurnitureTypeByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new furniture type for an order
    /// </summary>
    Task<FurnitureTypeDto> CreateFurnitureTypeAsync(CreateFurnitureTypeDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing furniture type
    /// </summary>
    Task<FurnitureTypeDto> UpdateFurnitureTypeAsync(int id, UpdateFurnitureTypeDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a furniture type
    /// </summary>
    Task DeleteFurnitureTypeAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all details across all furniture types assigned to the constructor
    /// </summary>
    Task<IEnumerable<DetailDto>> GetAllConstructorDetailsAsync(int constructorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all details for a furniture type
    /// </summary>
    Task<IEnumerable<DetailDto>> GetDetailsByFurnitureTypeAsync(int furnitureTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a detail by ID
    /// </summary>
    Task<DetailDto> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new detail for a furniture type
    /// </summary>
    Task<DetailDto> CreateDetailAsync(CreateDetailDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing detail
    /// </summary>
    Task<DetailDto> UpdateDetailAsync(int id, UpdateDetailDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a detail
    /// </summary>
    Task DeleteDetailAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all drawings across all furniture types assigned to the constructor
    /// </summary>
    Task<IEnumerable<DrawingDto>> GetAllConstructorDrawingsAsync(int constructorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a drawing file for a furniture type
    /// </summary>
    Task<DrawingDto> UploadDrawingAsync(int furnitureTypeId, IFormFile file, string? description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a drawing
    /// </summary>
    Task DeleteDrawingAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a technical specification for a furniture type
    /// </summary>
    Task<TechnicalSpecificationDto> CreateTechnicalSpecificationAsync(CreateTechnicalSpecificationDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes and locks a technical specification, making it ready for production
    /// </summary>
    Task CompleteTechnicalSpecificationAsync(int furnitureTypeId,CancellationToken cancellationToken = default);
}
