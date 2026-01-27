using FluentValidation;
using FurniFlowUz.Application.DTOs.Warehouse;

namespace FurniFlowUz.Application.Validators.Warehouse;

/// <summary>
/// Validator for CreateOutcomeTransactionDto
/// </summary>
public class CreateOutcomeTransactionDtoValidator : AbstractValidator<CreateOutcomeTransactionDto>
{
    public CreateOutcomeTransactionDtoValidator()
    {
        RuleFor(x => x.WarehouseItemId)
            .NotEmpty().WithMessage("Warehouse item ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(999999999).WithMessage("Quantity is too large.");

        RuleFor(x => x.TeamId)
            .NotEmpty().WithMessage("Team ID is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
