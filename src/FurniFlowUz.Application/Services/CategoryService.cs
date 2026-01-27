using AutoMapper;
using FurniFlowUz.Application.DTOs.Category;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for furniture category management
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto request, CancellationToken cancellationToken = default)
    {
        // Check if category name already exists
        var existingCategories = await _unitOfWork.Categories.FindAsync(
            c => c.Name.ToLower() == request.Name.ToLower(),
            cancellationToken);

        if (existingCategories.Any())
        {
            throw new ValidationException($"Category with name '{request.Name}' already exists.");
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto request, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }

        // Check if new name conflicts with existing categories
        if (category.Name != request.Name)
        {
            var existingCategories = await _unitOfWork.Categories.FindAsync(
                c => c.Name.ToLower() == request.Name.ToLower() && c.Id != id,
                cancellationToken);

            if (existingCategories.Any())
            {
                throw new ValidationException($"Category with name '{request.Name}' already exists.");
            }
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }

        // Check if category is used in contracts or orders
        var allContracts = await _unitOfWork.Contracts.GetAllAsync(cancellationToken);
        var contractsWithCategory = allContracts.Where(c => c.CategoryIds.Contains(id.ToString())).ToList();
        if (contractsWithCategory.Any())
        {
            throw new BusinessException("Cannot delete category with associated contracts.");
        }

        var orders = await _unitOfWork.Orders.FindAsync(o => o.CategoryId == id, cancellationToken);
        if (orders.Any())
        {
            throw new BusinessException("Cannot delete category with associated orders.");
        }

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
