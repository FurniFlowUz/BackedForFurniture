namespace FurniFlowUz.Application.DTOs.MaterialAssignment;

/// <summary>
/// Material Assignment DTO
/// </summary>
public class MaterialAssignmentDto
{
    /// <summary>
    /// Assignment ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Material request ID
    /// </summary>
    public int MaterialRequestId { get; set; }

    /// <summary>
    /// Material request number
    /// </summary>
    public string RequestNumber { get; set; } = string.Empty;

    /// <summary>
    /// Warehouse item ID
    /// </summary>
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Material name
    /// </summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>
    /// Material SKU
    /// </summary>
    public string MaterialSKU { get; set; } = string.Empty;

    /// <summary>
    /// Quantity assigned
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit of measure
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Assigned to team ID (nullable)
    /// </summary>
    public int? AssignedToTeamId { get; set; }

    /// <summary>
    /// Team name if assigned to team
    /// </summary>
    public string? TeamName { get; set; }

    /// <summary>
    /// Assigned to employee ID (nullable)
    /// </summary>
    public int? AssignedToEmployeeId { get; set; }

    /// <summary>
    /// Employee name if assigned to employee
    /// </summary>
    public string? EmployeeName { get; set; }

    /// <summary>
    /// Whether receipt was confirmed
    /// </summary>
    public bool ReceivedConfirmed { get; set; }

    /// <summary>
    /// When materials were received
    /// </summary>
    public DateTime? ReceivedAt { get; set; }

    /// <summary>
    /// Who confirmed receipt (user ID)
    /// </summary>
    public int? ReceivedByUserId { get; set; }

    /// <summary>
    /// Name of user who confirmed receipt
    /// </summary>
    public string? ReceivedByUserName { get; set; }

    /// <summary>
    /// Assignment date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last updated date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
