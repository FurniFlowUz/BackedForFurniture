using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Employee;

public class CreateEmployeeWithUserDto
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

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    public bool ForcePasswordChange { get; set; } = true;
}
