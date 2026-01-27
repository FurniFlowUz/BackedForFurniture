using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Employee;

public class UpdateEmployeeDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    public int PositionId { get; set; }

    [Required]
    public int DepartmentId { get; set; }
}
