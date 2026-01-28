using AutoMapper;
using FurniFlowUz.Application.DTOs.FurnitureTypeTemplate;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for furniture type template operations
/// </summary>
public class FurnitureTypeTemplateService : IFurnitureTypeTemplateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FurnitureTypeTemplateService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FurnitureTypeTemplateDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _unitOfWork.FurnitureTypeTemplates.GetAllAsync(cancellationToken);
        var filteredTemplates = templates.Where(t => !t.IsDeleted).OrderBy(t => t.DisplayOrder).ThenBy(t => t.Name);
        return _mapper.Map<IEnumerable<FurnitureTypeTemplateDto>>(filteredTemplates);
    }

    public async Task<FurnitureTypeTemplateDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.FurnitureTypeTemplates.GetByIdAsync(id, cancellationToken);
        if (template == null || template.IsDeleted)
        {
            throw new NotFoundException(nameof(FurnitureTypeTemplate), id);
        }

        return _mapper.Map<FurnitureTypeTemplateDto>(template);
    }

    public async Task<IEnumerable<FurnitureTypeTemplateDto>> GetActiveByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        // Verify category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
        if (category == null || category.IsDeleted)
        {
            throw new NotFoundException(nameof(Category), categoryId);
        }

        var templates = await _unitOfWork.FurnitureTypeTemplates.GetActiveByCategoryIdAsync(categoryId, cancellationToken);
        return _mapper.Map<IEnumerable<FurnitureTypeTemplateDto>>(templates);
    }

    public async Task<IEnumerable<FurnitureTypeTemplateDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        // Verify category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
        if (category == null || category.IsDeleted)
        {
            throw new NotFoundException(nameof(Category), categoryId);
        }

        var templates = await _unitOfWork.FurnitureTypeTemplates.GetByCategoryIdAsync(categoryId, cancellationToken);
        return _mapper.Map<IEnumerable<FurnitureTypeTemplateDto>>(templates);
    }

    public async Task<FurnitureTypeTemplateDto> CreateAsync(CreateFurnitureTypeTemplateDto request, CancellationToken cancellationToken = default)
    {
        // Verify category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null || category.IsDeleted)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var template = new FurnitureTypeTemplate
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            Description = request.Description,
            DefaultMaterial = request.DefaultMaterial,
            DefaultNotes = request.DefaultNotes,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FurnitureTypeTemplates.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FurnitureTypeTemplateDto>(template);
    }

    public async Task<FurnitureTypeTemplateDto> UpdateAsync(int id, UpdateFurnitureTypeTemplateDto request, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.FurnitureTypeTemplates.GetByIdAsync(id, cancellationToken);
        if (template == null || template.IsDeleted)
        {
            throw new NotFoundException(nameof(FurnitureTypeTemplate), id);
        }

        template.Name = request.Name;
        template.Description = request.Description;
        template.DefaultMaterial = request.DefaultMaterial;
        template.DefaultNotes = request.DefaultNotes;
        template.IsActive = request.IsActive;
        template.DisplayOrder = request.DisplayOrder;
        template.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.FurnitureTypeTemplates.Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FurnitureTypeTemplateDto>(template);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.FurnitureTypeTemplates.GetByIdAsync(id, cancellationToken);
        if (template == null || template.IsDeleted)
        {
            throw new NotFoundException(nameof(FurnitureTypeTemplate), id);
        }

        // Soft delete
        template.IsDeleted = true;
        template.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.FurnitureTypeTemplates.Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<FurnitureTypeTemplateDto> ToggleActiveStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.FurnitureTypeTemplates.GetByIdAsync(id, cancellationToken);
        if (template == null || template.IsDeleted)
        {
            throw new NotFoundException(nameof(FurnitureTypeTemplate), id);
        }

        template.IsActive = !template.IsActive;
        template.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.FurnitureTypeTemplates.Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FurnitureTypeTemplateDto>(template);
    }
}
