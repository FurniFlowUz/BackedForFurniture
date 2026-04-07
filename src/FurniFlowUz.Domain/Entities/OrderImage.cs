using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

/// <summary>
/// Entity for storing order images (room photos and design references)
/// </summary>
public class OrderImage : BaseAuditableEntity
{
    /// <summary>
    /// Order ID this image belongs to
    /// </summary>
    [Required]
    public int OrderId { get; set; }

    /// <summary>
    /// Original file name
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Full path to the saved file
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MIME content type (e.g., image/jpeg, image/png)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Type of image: "room" or "design"
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string ImageType { get; set; } = string.Empty;

    /// <summary>
    /// When the image was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// User ID who uploaded this image
    /// </summary>
    public int? UploadedBy { get; set; }

    // Navigation properties
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(UploadedBy))]
    public User? Uploader { get; set; }
}
