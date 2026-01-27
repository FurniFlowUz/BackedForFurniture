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
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ExpectedDeliveryDate)
            .NotEmpty().WithMessage("Expected delivery date is required.");

        RuleFor(x => x.ActualDeliveryDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-10)).WithMessage("Actual delivery date is invalid.")
            .When(x => x.ActualDeliveryDate.HasValue);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(BeValidStatus).WithMessage("Invalid status. Valid statuses are: Pending, InProgress, Completed, Cancelled, New.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }

    private bool BeValidStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return false;
        var validStatuses = new[] { "Pending", "InProgress", "Completed", "Cancelled", "New" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
