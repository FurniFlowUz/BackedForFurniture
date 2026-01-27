using FluentValidation;
using FurniFlowUz.Application.DTOs.Warehouse;

namespace FurniFlowUz.Application.Validators.Warehouse;

/// <summary>
/// Validator for ConfirmMaterialRequestDto
/// </summary>
public class ConfirmMaterialRequestDtoValidator : AbstractValidator<ConfirmMaterialRequestDto>
{
    public ConfirmMaterialRequestDtoValidator()
    {
        RuleFor(x => x.MaterialRequestId)
            .NotEmpty().WithMessage("Material request ID is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
