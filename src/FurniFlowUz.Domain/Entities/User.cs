using System.ComponentModel.DataAnnotations;
using FurniFlowUz.Domain.Common;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Domain.Entities;

public class User : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public Employee? Employee { get; set; }
    public ICollection<Order> AssignedOrdersAsConstructor { get; set; } = new List<Order>();
    public ICollection<Order> AssignedOrdersAsProductionManager { get; set; } = new List<Order>();
    public ICollection<Team> LeadingTeams { get; set; } = new List<Team>();
    public ICollection<Team> TeamMemberships { get; set; } = new List<Team>();
    public ICollection<WorkTask> AssignedTasks { get; set; } = new List<WorkTask>();
    public ICollection<WarehouseTransaction> WarehouseTransactions { get; set; } = new List<WarehouseTransaction>();
    public ICollection<MaterialRequest> MaterialRequests { get; set; } = new List<MaterialRequest>();
    public ICollection<MaterialRequest> ConfirmedMaterialRequests { get; set; } = new List<MaterialRequest>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
