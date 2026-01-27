using FluentValidation;
using FurniFlowUz.Application.DTOs.Category;

namespace FurniFlowUz.Application.Validators.Category;

/// <summary>
/// Validator for UpdateCategoryDto
/// </summary>
public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.StandardPreparationDays)
            .GreaterThan(0).WithMessage("Standard preparation days must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("Standard preparation days cannot exceed 365 days.");
    }
}
