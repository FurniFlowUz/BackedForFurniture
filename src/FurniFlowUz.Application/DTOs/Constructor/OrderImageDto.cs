namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// DTO for order image response
/// </summary>
public class OrderImageDto
{
    /// <summary>
    /// Image ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// URL to access the image file
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Type of image: "room" or "design"
    /// </summary>
    public string ImageType { get; set; } = string.Empty;

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// When the image was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; }
}
