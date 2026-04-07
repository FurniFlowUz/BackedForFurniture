using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.FurnitureTypeTemplate;

/// <summary>
/// DTO for updating furniture type template
/// </summary>
public class UpdateFurnitureTypeTemplateDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? DefaultMaterial { get; set; }

    [MaxLength(2000)]
    public string? DefaultNotes { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;
}
