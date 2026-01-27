using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Department;

public class CreateDepartmentDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
