using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// DTO for completing (locking) a technical specification
/// </summary>
public class CompleteTechnicalSpecDto
{
    /// <summary>
    /// Furniture type identifier whose specification should be locked
    /// </summary>
    [Required(ErrorMessage = "Furniture type ID is required")]
    public int? FurnitureTypeId { get; set; }
}
