using FluentValidation;
using FurniFlowUz.Application.DTOs.Warehouse;

namespace FurniFlowUz.Application.Validators.Warehouse;

/// <summary>
/// Validator for UpdateWarehouseItemDto
/// </summary>
public class UpdateWarehouseItemDtoValidator : AbstractValidator<UpdateWarehouseItemDto>
{
    public UpdateWarehouseItemDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(200).WithMessage("Item name cannot exceed 200 characters.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.")
            .Matches(@"^[A-Za-z0-9\-_]+$").WithMessage("SKU can only contain letters, numbers, hyphens, and underscores.");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock must be non-negative.")
            .LessThanOrEqualTo(999999999).WithMessage("Minimum stock is too large.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit of measurement is required.")
            .Must(BeValidUnit).WithMessage("Invalid unit. Valid units are: Piece, Kilogram, Meter, SquareMeter, CubicMeter, Liter.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price must be non-negative.")
            .LessThanOrEqualTo(999999999).WithMessage("Unit price is too large.");

        RuleFor(x => x.Supplier)
            .MaximumLength(200).WithMessage("Supplier name cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Supplier));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    private bool BeValidUnit(string unit)
    {
        if (string.IsNullOrWhiteSpace(unit)) return false;
        var validUnits = new[] { "Piece", "Kilogram", "Meter", "SquareMeter", "CubicMeter", "Liter", "pcs", "kg", "m", "m2", "m3", "l" };
        return validUnits.Contains(unit, StringComparer.OrdinalIgnoreCase);
    }
}
