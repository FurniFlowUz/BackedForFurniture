using FurniFlowUz.Application.DTOs.Auth;
using FurniFlowUz.Application.DTOs.Production;

namespace FurniFlowUz.Application.DTOs.Warehouse;

/// <summary>
/// Material request details DTO
/// </summary>
public class MaterialRequestDto
{
    /// <summary>
    /// Unique request identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Warehouse transaction ID
    /// </summary>
    public int WarehouseTransactionId { get; set; }

    /// <summary>
    /// Warehouse transaction details
    /// </summary>
    public WarehouseTransactionDto? WarehouseTransaction { get; set; }

    /// <summary>
    /// Team ID making the request
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Team name making the request
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// User ID who created the request
    /// </summary>
    public int RequestedByUserId { get; set; }

    /// <summary>
    /// User name who created the request
    /// </summary>
    public string RequestedByUserName { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation status (Pending, Approved, Rejected)
    /// </summary>
    public string ConfirmationStatus { get; set; } = string.Empty;

    /// <summary>
    /// User ID who confirmed the request
    /// </summary>
    public int? ConfirmedByUserId { get; set; }

    /// <summary>
    /// User name who confirmed the request
    /// </summary>
    public string? ConfirmedByUserName { get; set; }

    /// <summary>
    /// Date when request was confirmed
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Request notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date when request was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
