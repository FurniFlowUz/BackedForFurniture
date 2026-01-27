using FluentValidation;
using FurniFlowUz.Application.DTOs.Constructor;

namespace FurniFlowUz.Application.Validators.Constructor;

/// <summary>
/// Validator for CreateFurnitureTypeDto
/// </summary>
public class CreateFurnitureTypeDtoValidator : AbstractValidator<CreateFurnitureTypeDto>
{
    public CreateFurnitureTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Furniture type name is required.")
            .MaximumLength(200).WithMessage("Furniture type name cannot exceed 200 characters.");

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");
    }
}
