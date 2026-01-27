using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// DTO for assigning an order to a constructor or production manager
/// </summary>
public class AssignOrderDto
{
    /// <summary>
    /// Order identifier
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }

    /// <summary>
    /// Constructor identifier (optional, use either this or ProductionManagerId)
    /// </summary>
    public int? ConstructorId { get; set; }

    /// <summary>
    /// Production manager identifier (optional, use either this or ConstructorId)
    /// </summary>
    public int? ProductionManagerId { get; set; }
}
