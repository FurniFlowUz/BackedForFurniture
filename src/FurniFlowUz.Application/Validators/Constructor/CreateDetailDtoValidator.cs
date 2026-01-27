using FluentValidation;
using FurniFlowUz.Application.DTOs.Constructor;

namespace FurniFlowUz.Application.Validators.Constructor;

/// <summary>
/// Validator for CreateDetailDto
/// </summary>
public class CreateDetailDtoValidator : AbstractValidator<CreateDetailDto>
{
    public CreateDetailDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Detail name is required.")
            .MaximumLength(200).WithMessage("Detail name cannot exceed 200 characters.");

        RuleFor(x => x.Width)
            .GreaterThan(0).WithMessage("Width must be greater than 0.")
            .LessThanOrEqualTo(99999).WithMessage("Width is too large.");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0.")
            .LessThanOrEqualTo(99999).WithMessage("Height is too large.");

        RuleFor(x => x.Thickness)
            .GreaterThan(0).WithMessage("Thickness must be greater than 0.")
            .LessThanOrEqualTo(9999).WithMessage("Thickness is too large.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(10000).WithMessage("Quantity is too large.");

        RuleFor(x => x.FurnitureTypeId)
            .NotEmpty().WithMessage("Furniture type ID is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
