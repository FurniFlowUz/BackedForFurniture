using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.MaterialAssignment;

/// <summary>
/// DTO for confirming material receipt
/// </summary>
public class ConfirmMaterialReceiptDto
{
    /// <summary>
    /// Material assignment ID
    /// </summary>
    [Required(ErrorMessage = "Assignment ID is required")]
    public int AssignmentId { get; set; }

    /// <summary>
    /// Optional notes for receipt confirmation
    /// </summary>
    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
