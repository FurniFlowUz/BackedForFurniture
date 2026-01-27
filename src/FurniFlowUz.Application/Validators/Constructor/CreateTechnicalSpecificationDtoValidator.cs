using FluentValidation;
using FurniFlowUz.Application.DTOs.Constructor;

namespace FurniFlowUz.Application.Validators.Constructor;

/// <summary>
/// Validator for CreateTechnicalSpecificationDto
/// </summary>
public class CreateTechnicalSpecificationDtoValidator : AbstractValidator<CreateTechnicalSpecificationDto>
{
    public CreateTechnicalSpecificationDtoValidator()
    {
        RuleFor(x => x.FurnitureTypeId)
            .NotEmpty().WithMessage("Furniture type ID is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(5000).WithMessage("Notes cannot exceed 5000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
