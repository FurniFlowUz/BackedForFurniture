using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FurniFlowUz.Application.DTOs.Constructor;

/// <summary>
/// Request DTO for uploading an order image
/// </summary>
public class UploadOrderImageRequest
{
    /// <summary>
    /// Order ID to attach the image to
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    public int OrderId { get; set; }

    /// <summary>
    /// Type of image: "room" or "design"
    /// </summary>
    [Required(ErrorMessage = "Image type is required")]
    [RegularExpression("^(room|design)$", ErrorMessage = "Image type must be 'room' or 'design'")]
    public string ImageType { get; set; } = string.Empty;

    /// <summary>
    /// The image file to upload
    /// </summary>
    [Required(ErrorMessage = "File is required")]
    public IFormFile File { get; set; } = null!;
}
