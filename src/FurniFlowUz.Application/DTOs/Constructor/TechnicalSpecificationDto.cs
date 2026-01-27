using FurniFlowUz.Application.DTOs.Auth;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// Technical specification details DTO
/// </summary>
public class TechnicalSpecificationDto
{
    /// <summary>
    /// Unique specification identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Associated furniture type identifier
    /// </summary>
    public int? FurnitureTypeId { get; set; }

    /// <summary>
    /// Indicates if the specification is locked (approved)
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Additional notes or specifications
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Constructor who created the specification
    /// </summary>
    public UserDto? CreatedBy { get; set; }

    /// <summary>
    /// Date and time when specification was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when specification was locked
    /// </summary>
    public DateTime? LockedAt { get; set; }

    /// <summary>
    /// User who locked the specification
    /// </summary>
    public UserDto? LockedBy { get; set; }
}
