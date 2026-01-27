using FluentValidation;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.Validators.Production;

/// <summary>
/// Validator for CreateWorkTaskDto
/// </summary>
public class CreateWorkTaskDtoValidator : AbstractValidator<CreateWorkTaskDto>
{
    public CreateWorkTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Task description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.ProductionStageId)
            .NotEmpty().WithMessage("Production stage ID is required.");

        RuleFor(x => x.TeamId)
            .NotEmpty().WithMessage("Team ID is required.");

        RuleFor(x => x.SequenceOrder)
            .GreaterThan(0).WithMessage("Sequence order must be greater than 0.")
            .LessThanOrEqualTo(1000).WithMessage("Sequence order cannot exceed 1000.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("Estimated hours must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Estimated hours is too large.");
    }
}
