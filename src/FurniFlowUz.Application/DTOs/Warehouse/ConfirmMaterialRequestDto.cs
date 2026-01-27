using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// DTO for confirming (approving or rejecting) a material request
/// </summary>
public class ConfirmMaterialRequestDto
{
    /// <summary>
    /// Material request identifier
    /// </summary>
    [Required(ErrorMessage = "Material request ID is required")]
    public int? MaterialRequestId { get; set; }

    /// <summary>
    /// Whether the request is accepted (true) or rejected (false)
    /// </summary>
    [Required(ErrorMessage = "IsAccepted is required")]
    public bool IsAccepted { get; set; }

    /// <summary>
    /// Response notes from warehouseman
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}
