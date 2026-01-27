using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for notification management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all notifications for the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user notifications</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetNotifications(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(notifications, "Notifications retrieved successfully"));
    }

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("{id}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _notificationService.MarkAsReadAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Notification marked as read"));
    }

    /// <summary>
    /// Marks all notifications as read for the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead(
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "All notifications marked as read"));
    }

    /// <summary>
    /// Deletes a notification
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNotification(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _notificationService.DeleteNotificationAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Notification deleted successfully"));
    }
}
