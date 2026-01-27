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
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.ActualHours)
            .GreaterThan(0).WithMessage("Actual hours must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Actual hours is too large.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
