using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// DTO for assigning a production manager to an order
/// Order ID comes from route, NOT from body
/// </summary>
public class AssignProductionManagerDto
{
    /// <summary>
    /// Production Manager identifier (Employee ID)
    /// </summary>
    [Required(ErrorMessage = "Production Manager ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Production Manager ID must be greater than 0")]
    public int ProductionManagerId { get; set; }
}
