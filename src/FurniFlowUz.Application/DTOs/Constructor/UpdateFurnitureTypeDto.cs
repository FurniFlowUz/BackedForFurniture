using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// DTO for updating a furniture type
/// </summary>
public class UpdateFurnitureTypeDto
{
    /// <summary>
    /// Furniture type name
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
