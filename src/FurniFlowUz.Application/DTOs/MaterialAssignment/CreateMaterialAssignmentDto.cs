using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.MaterialAssignment;

/// <summary>
/// DTO for creating a new material assignment
/// </summary>
public class CreateMaterialAssignmentDto
{
    /// <summary>
    /// Material request ID
    /// </summary>
    [Required(ErrorMessage = "Material Request ID is required")]
    public int MaterialRequestId { get; set; }

    /// <summary>
    /// Warehouse item ID
    /// </summary>
    [Required(ErrorMessage = "Warehouse Item ID is required")]
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Quantity to assign
    /// </summary>
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Assign to team ID (use this OR AssignedToEmployeeId, not both)
    /// </summary>
    public int? AssignedToTeamId { get; set; }

    /// <summary>
    /// Assign to employee ID (use this OR AssignedToTeamId, not both)
    /// </summary>
    public int? AssignedToEmployeeId { get; set; }
}
