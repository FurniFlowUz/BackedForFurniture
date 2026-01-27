using FluentValidation;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.Validators.Production;

/// <summary>
/// Validator for UpdateWorkTaskDto
/// </summary>
public class UpdateWorkTaskDtoValidator : AbstractValidator<UpdateWorkTaskDto>
{
    public UpdateWorkTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Task description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("Estimated hours must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Estimated hours is too large.");
    }
}
