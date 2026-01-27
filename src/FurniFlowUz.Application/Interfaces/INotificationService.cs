using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for notification management
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Gets all notifications for a specific user
    /// </summary>
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all notifications for users with a specific role
    /// </summary>
    Task<IEnumerable<NotificationDto>> GetRoleNotificationsAsync(UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new notification
    /// </summary>
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all notifications as read for a user
    /// </summary>
    Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a notification
    /// </summary>
    Task DeleteNotificationAsync(int notificationId, CancellationToken cancellationToken = default);
}
