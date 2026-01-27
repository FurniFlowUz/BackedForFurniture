using FluentValidation;
using FurniFlowUz.Application.DTOs.Category;

namespace FurniFlowUz.Application.Validators.Category;

/// <summary>
/// Validator for CreateCategoryDto
/// </summary>
public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.RetailPrice)
            .GreaterThan(0).WithMessage("Retail price must be greater than 0.");

        RuleFor(x => x.WholesalePrice)
            .GreaterThan(0).WithMessage("Wholesale price must be greater than 0.");

        RuleFor(x => x.MinAdvancePercent)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum advance percent must be 0 or greater.")
            .LessThanOrEqualTo(100).WithMessage("Minimum advance percent cannot exceed 100.");

        RuleFor(x => x.StandardProductionDays)
            .GreaterThan(0).WithMessage("Standard production days must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("Standard production days cannot exceed 365 days.");
    }
}
