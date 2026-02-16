using FluentValidation;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.Validators.Production;

/// <summary>
/// Validator for CompleteTaskDto
/// </summary>
public class CompleteTaskDtoValidator : AbstractValidator<CompleteTaskDto>
{
    public CompleteTaskDtoValidator()
    {
        // ActualHours is optional, but if provided must be valid
        RuleFor(x => x.ActualHours)
            .GreaterThanOrEqualTo(0).WithMessage("Actual hours must be 0 or greater.")
            .LessThanOrEqualTo(10000).WithMessage("Actual hours is too large.")
            .When(x => x.ActualHours.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
