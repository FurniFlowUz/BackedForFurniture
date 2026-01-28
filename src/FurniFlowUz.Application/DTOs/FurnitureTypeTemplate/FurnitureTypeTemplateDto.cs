namespace FurniFlowUz.Application.DTOs.FurnitureTypeTemplate;

/// <summary>
/// DTO for furniture type template
/// </summary>
public class FurnitureTypeTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public string? DefaultMaterial { get; set; }
    public string? DefaultNotes { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
