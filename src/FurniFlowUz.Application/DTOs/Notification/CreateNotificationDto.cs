using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Notification;

/// <summary>
/// DTO for creating a new notification
/// </summary>
public class CreateNotificationDto
{
    /// <summary>
    /// Notification type (Info, Warning, Error, Success)
    /// </summary>
    [Required(ErrorMessage = "Type is required")]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Notification title
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification message
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// User identifier who should receive this notification (optional, use either UserId or Role)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Role that should receive this notification (optional, use either UserId or Role)
    /// </summary>
    [MaxLength(50)]
    public string? Role { get; set; }

    /// <summary>
    /// Related entity type (Order, Task, Contract, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Related entity identifier
    /// </summary>
    public int? RelatedEntityId { get; set; }
}
