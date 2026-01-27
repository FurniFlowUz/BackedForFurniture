using FluentValidation;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.Validators.Production;

/// <summary>
/// Validator for AssignTaskDto
/// </summary>
public class AssignTaskDtoValidator : AbstractValidator<AssignTaskDto>
{
    public AssignTaskDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.WorkerId)
            .NotEmpty().WithMessage("Worker ID is required.");
    }
}
