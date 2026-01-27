using FluentValidation;
using FurniFlowUz.Application.DTOs.Constructor;

namespace FurniFlowUz.Application.Validators.Constructor;

/// <summary>
/// Validator for UpdateFurnitureTypeDto
/// </summary>
public class UpdateFurnitureTypeDtoValidator : AbstractValidator<UpdateFurnitureTypeDto>
{
    public UpdateFurnitureTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Furniture type name is required.")
            .MaximumLength(200).WithMessage("Furniture type name cannot exceed 200 characters.");
    }
}
