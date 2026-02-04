using FluentValidation;
using FurniFlowUz.Application.DTOs.Contract;

namespace FurniFlowUz.Application.Validators.Contract;

/// <summary>
/// Validator for UpdateContractDto
/// </summary>
public class UpdateContractDtoValidator : AbstractValidator<UpdateContractDto>
{
    public UpdateContractDtoValidator()
    {
        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("At least one category is required.")
            .Must(list => list != null && list.Count > 0).WithMessage("At least one category is required.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0.")
            .LessThanOrEqualTo(999999999).WithMessage("Total amount is too large.");

        RuleFor(x => x.AdvancePaymentAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Advance payment amount must be non-negative.")
            .LessThanOrEqualTo(x => x.TotalAmount).WithMessage("Advance payment amount cannot exceed total amount.");

        RuleFor(x => x.DeadlineDate)
            .NotEmpty().WithMessage("Deadline date is required.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(BeValidStatus).WithMessage("Invalid status. Valid statuses are: New, Active, Completed, Cancelled.");

        RuleFor(x => x.DeliveryTerms)
            .MaximumLength(2000).WithMessage("Delivery terms cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.DeliveryTerms));

        RuleFor(x => x.PenaltyTerms)
            .MaximumLength(2000).WithMessage("Penalty terms cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.PenaltyTerms));

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(2000).WithMessage("Additional notes cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.AdditionalNotes));
    }

    private bool BeValidStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return false;
        var validStatuses = new[] { "Draft", "Active", "Completed", "Cancelled", "New", "InProgress" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
