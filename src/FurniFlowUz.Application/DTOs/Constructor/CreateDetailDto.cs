using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// DTO for creating a new detail (component)
/// </summary>
public class CreateDetailDto
{
    /// <summary>
    /// Detail name
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Width in millimeters
    /// </summary>
    [Required(ErrorMessage = "Width is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Width must be greater than 0")]
    public decimal Width { get; set; }

    /// <summary>
    /// Height in millimeters
    /// </summary>
    [Required(ErrorMessage = "Height is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0")]
    public decimal Height { get; set; }

    /// <summary>
    /// Thickness in millimeters
    /// </summary>
    [Required(ErrorMessage = "Thickness is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Thickness must be greater than 0")]
    public decimal Thickness { get; set; }

    /// <summary>
    /// Quantity needed
    /// </summary>
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    /// <summary>
    /// Associated furniture type identifier
    /// </summary>
    [Required(ErrorMessage = "Furniture type ID is required")]
    public int FurnitureTypeId { get; set; }

    /// <summary>
    /// Material type
    /// </summary>
    [MaxLength(200)]
    public string? Material { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}
