using FurniFlowUz.Application.DTOs.Auth;

namespace FurniFlowUz.Application.DTOs.Notification;

/// <summary>
/// Full notification details DTO
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// Unique notification identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Notification type (Info, Warning, Error, Success)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Notification title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// User ID who should receive this notification (null for role-based notifications)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// User full name who should receive this notification
    /// </summary>
    public string? UserFullName { get; set; }

    /// <summary>
    /// Role that should receive this notification (null for user-specific notifications)
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Related entity type (Order, Task, Contract, etc.)
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Related entity identifier
    /// </summary>
    public int? RelatedEntityId { get; set; }

    /// <summary>
    /// Indicates if the notification has been read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Date when notification was read
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Date and time when notification was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
