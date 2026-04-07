using FluentValidation;
using FurniFlowUz.Application.DTOs.Contract;

namespace FurniFlowUz.Application.Validators.Contract;

/// <summary>
/// Validator for CreateContractDto
/// </summary>
public class CreateContractDtoValidator : AbstractValidator<CreateContractDto>
{
    public CreateContractDtoValidator()
    {
        // CRITICAL: Either CustomerId OR NewCustomer must be provided, not both, not none
        RuleFor(x => x)
            .Must(HaveValidCustomerInput)
            .WithMessage("Either CustomerId or NewCustomer must be provided, but not both.");

        // Validate CustomerId when provided (must be > 0)
        When(x => x.CustomerId.HasValue, () =>
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("CustomerId must be greater than 0 when provided.");
        });

        // Validate NewCustomer when provided
        When(x => x.NewCustomer != null, () =>
        {
            RuleFor(x => x.NewCustomer!.FullName)
                .NotEmpty().WithMessage("Customer full name is required.")
                .MinimumLength(2).WithMessage("Customer full name must be at least 2 characters.")
                .MaximumLength(200).WithMessage("Customer full name cannot exceed 200 characters.");

            RuleFor(x => x.NewCustomer!.PhoneNumber)
                .NotEmpty().WithMessage("Customer phone number is required.")
                .Matches(@"^\+?[\d\s\-()]+$").WithMessage("Invalid phone number format.")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

            RuleFor(x => x.NewCustomer!.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.")
                .When(x => !string.IsNullOrEmpty(x.NewCustomer?.Email));

            RuleFor(x => x.NewCustomer!.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.NewCustomer?.Address));

            RuleFor(x => x.NewCustomer!.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.NewCustomer?.Notes));
        });

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
            .NotEmpty().WithMessage("Deadline date is required.")
            .GreaterThan(DateTime.UtcNow.Date).WithMessage("Deadline date must be in the future.");

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

    /// <summary>
    /// Validates that exactly one customer input method is provided
    /// </summary>
    private bool HaveValidCustomerInput(CreateContractDto dto)
    {
        bool hasCustomerId = dto.CustomerId.HasValue && dto.CustomerId.Value > 0;
        bool hasNewCustomer = dto.NewCustomer != null;

        // XOR: Exactly one must be true
        return hasCustomerId ^ hasNewCustomer;
    }
}
