namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// Drawing information DTO
/// </summary>
public class DrawingDto
{
    /// <summary>
    /// Unique drawing identifier
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Drawing file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Drawing file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Content type (MIME type)
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Associated furniture type identifier
    /// </summary>
    public int? FurnitureTypeId { get; set; }

    /// <summary>
    /// Description or notes about the drawing
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date and time when drawing was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; }
}
