using FluentValidation;
using FurniFlowUz.Application.DTOs.Warehouse;

namespace FurniFlowUz.Application.Validators.Warehouse;

/// <summary>
/// Validator for CreateIncomeTransactionDto
/// </summary>
public class CreateIncomeTransactionDtoValidator : AbstractValidator<CreateIncomeTransactionDto>
{
    public CreateIncomeTransactionDtoValidator()
    {
        RuleFor(x => x.WarehouseItemId)
            .NotEmpty().WithMessage("Warehouse item ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(999999999).WithMessage("Quantity is too large.");

        RuleFor(x => x.Supplier)
            .NotEmpty().WithMessage("Supplier is required.")
            .MaximumLength(200).WithMessage("Supplier name cannot exceed 200 characters.");

        RuleFor(x => x.InvoiceNumber)
            .MaximumLength(100).WithMessage("Invoice number cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.InvoiceNumber));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
