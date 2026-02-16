namespace FurniFlowUz.Application.DTOs.Production;

/// <summary>
/// Production stage information DTO
/// </summary>
public class ProductionStageDto
{
    /// <summary>
    /// Unique stage identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Stage name (e.g., Cutting, Assembly, Finishing, Quality Check)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Stage description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Display order for the stage
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Sequence order in production pipeline
    /// </summary>
    public int SequenceOrder { get; set; }

    /// <summary>
    /// Stage type (Sawing, Routing, EdgeBanding, Sanding, Assembly, Finishing, Painting, QualityControl)
    /// </summary>
    public string StageType { get; set; } = string.Empty;

    /// <summary>
    /// Estimated duration in hours
    /// </summary>
    public decimal? EstimatedDurationHours { get; set; }

    /// <summary>
    /// Indicates if the stage is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date and time when stage was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
