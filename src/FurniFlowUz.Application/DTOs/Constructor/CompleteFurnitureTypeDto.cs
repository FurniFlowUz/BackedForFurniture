using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// DTO for completing a furniture type with all data
/// </summary>
public class CompleteFurnitureTypeDto
{
    /// <summary>
    /// List of details to be created
    /// </summary>
    public List<DetailItemDto>? Details { get; set; }

    /// <summary>
    /// Technical specification notes
    /// </summary>
    [MaxLength(5000)]
    public string? Notes { get; set; }
}

/// <summary>
/// Detail item for bulk creation
/// </summary>
public class DetailItemDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Width { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Height { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Thickness { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [MaxLength(200)]
    public string? Material { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
