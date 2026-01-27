using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// DTO for creating a technical specification
/// </summary>
public class CreateTechnicalSpecificationDto
{
    /// <summary>
    /// Associated furniture type identifier
    /// </summary>
    [Required(ErrorMessage = "Furniture type ID is required")]
    public int? FurnitureTypeId { get; set; }

    /// <summary>
    /// Additional notes or specifications
    /// </summary>
    [MaxLength(5000)]
    public string? Notes { get; set; }
}
