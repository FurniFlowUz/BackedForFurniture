using FluentValidation;
using FurniFlowUz.Application.DTOs.Auth;
using System.Text.RegularExpressions;

namespace FurniFlowUz.Application.Validators.Auth;

/// <summary>
/// Validator for RegisterRequestDto
/// </summary>
public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.")
            .Must(BeValidName).WithMessage("First name contains invalid characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
            .Must(BeValidName).WithMessage("Last name contains invalid characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
            .Must(BeValidPhoneNumber).WithMessage("Invalid phone number format. Use formats like +998901234567 or 998901234567.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
            .Must(BeStrongPassword).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one digit.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(BeValidRole).WithMessage("Invalid role. Valid roles are: Director, ProductionManager, Constructor, Worker, Warehouseman.");
    }

    private bool BeValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        // Allow letters, spaces, hyphens, and apostrophes
        return Regex.IsMatch(name, @"^[a-zA-Z\s\-']+$");
    }

    private bool BeValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
        // Allow + sign, digits, spaces, hyphens, and parentheses
        return Regex.IsMatch(phoneNumber, @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$");
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

    private bool BeValidRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role)) return false;
        var validRoles = new[] { "Director", "ProductionManager", "Constructor", "Worker", "Warehouseman" };
        return validRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
