using FluentValidation;
using FurniFlowUz.Application.DTOs.Customer;
using System.Text.RegularExpressions;

namespace FurniFlowUz.Application.Validators.Customer;

/// <summary>
/// Validator for UpdateCustomerDto
/// </summary>
public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(200).WithMessage("First name cannot exceed 200 characters.")
            .Must(BeValidName).WithMessage("First name contains invalid characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(200).WithMessage("Last name cannot exceed 200 characters.")
            .Must(BeValidName).WithMessage("Last name contains invalid characters.");

        RuleFor(x => x.CompanyName)
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.CompanyName));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
            .Must(BeValidPhoneNumber).WithMessage("Invalid phone number format. Use formats like +998901234567 or 998901234567.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.TaxNumber)
            .MaximumLength(50).WithMessage("Tax number cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.TaxNumber));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
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
}
