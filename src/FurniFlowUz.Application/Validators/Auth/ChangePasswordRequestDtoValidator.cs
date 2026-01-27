using FluentValidation;
using FurniFlowUz.Application.DTOs.Auth;
using System.Text.RegularExpressions;

namespace FurniFlowUz.Application.Validators.Auth;

/// <summary>
/// Validator for ChangePasswordRequestDto
/// </summary>
public class ChangePasswordRequestDtoValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("New password cannot exceed 100 characters.")
            .Must(BeStrongPassword).WithMessage("New password must contain at least one uppercase letter, one lowercase letter, and one digit.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }

    private bool BeStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        // Must contain at least one uppercase, one lowercase, and one digit
        bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
        bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
        bool hasDigit = Regex.IsMatch(password, @"[0-9]");
        return hasUpperCase && hasLowerCase && hasDigit;
    }
}
