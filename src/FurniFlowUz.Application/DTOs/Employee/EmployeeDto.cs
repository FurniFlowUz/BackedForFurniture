namespace FurniFlowUz.Application.DTOs.Employee;

public class EmployeeDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int PositionId { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ActiveTasks { get; set; }
    public int CompletedTasks { get; set; }
    public decimal? OnTimePercent { get; set; }
    public DateTime CreatedAt { get; set; }
}
