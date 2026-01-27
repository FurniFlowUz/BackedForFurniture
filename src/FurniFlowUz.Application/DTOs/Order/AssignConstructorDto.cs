using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Order;

/// <summary>
/// DTO for assigning a constructor to an order
/// Order ID comes from route, NOT from body
/// </summary>
public class AssignConstructorDto
{
    /// <summary>
    /// Constructor identifier (Employee ID)
    /// </summary>
    [Required(ErrorMessage = "Constructor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Constructor ID must be greater than 0")]
    public int ConstructorId { get; set; }
}
