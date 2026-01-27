using System.ComponentModel.DataAnnotations;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class Department : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
