using FluentValidation;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.Validators.Production;

/// <summary>
/// Validator for CreateTeamDto
/// </summary>
public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto>
{
    public CreateTeamDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required.")
            .MaximumLength(100).WithMessage("Team name cannot exceed 100 characters.");

        RuleFor(x => x.TeamLeaderId)
            .NotEmpty().WithMessage("Team leader is required.");

        RuleFor(x => x.MemberIds)
            .Must(members => members == null || members.Count <= 50)
            .WithMessage("A team cannot have more than 50 members.");
    }
}
