using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class Team : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int TeamLeaderId { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation properties
    [ForeignKey(nameof(TeamLeaderId))]
    public User TeamLeader { get; set; } = null!;

    public ICollection<User> Members { get; set; } = new List<User>();
    public ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();
    public ICollection<WarehouseTransaction> WarehouseTransactions { get; set; } = new List<WarehouseTransaction>();
    public ICollection<MaterialRequest> MaterialRequests { get; set; } = new List<MaterialRequest>();
}
