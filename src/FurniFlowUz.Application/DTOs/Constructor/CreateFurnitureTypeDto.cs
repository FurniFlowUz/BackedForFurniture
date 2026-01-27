using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// DTO for creating a new furniture type
/// </summary>
public class CreateFurnitureTypeDto
{
    /// <summary>
    /// Furniture type name
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Associated order identifier
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }
}
