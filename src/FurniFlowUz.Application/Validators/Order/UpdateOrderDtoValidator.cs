using FluentValidation;
using FurniFlowUz.Application.DTOs.Order;

namespace FurniFlowUz.Application.Validators.Order;

/// <summary>
/// Validator for UpdateOrderDto
/// </summary>
public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.CategoryIds)
            .Must(ids => ids == null || ids.All(id => id > 0))
            .WithMessage("All category IDs must be greater than 0.")
            .When(x => x.CategoryIds != null && x.CategoryIds.Any());
    }
}
