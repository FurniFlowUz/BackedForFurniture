using FluentValidation;
using FurniFlowUz.Application.DTOs.Order;

namespace FurniFlowUz.Application.Validators.Order;

/// <summary>
/// Validator for CreateOrderDto
/// </summary>
public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ContractId)
            .GreaterThan(0).WithMessage("Contract ID must be greater than 0.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
