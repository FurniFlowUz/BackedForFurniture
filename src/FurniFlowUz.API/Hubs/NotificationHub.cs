using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FurniFlowUz.API.Hubs;

/// <summary>
/// SignalR hub for real-time notification delivery
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// Adds the user to their role group for role-based notifications
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var role = Context.User?.FindFirst("role")?.Value;

        if (!string.IsNullOrEmpty(role))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, role);
            _logger.LogInformation("User {UserId} with role {Role} connected to NotificationHub", userId, role);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// Removes the user from their role group
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        var role = Context.User?.FindFirst("role")?.Value;

        if (!string.IsNullOrEmpty(role))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, role);
            _logger.LogInformation("User {UserId} with role {Role} disconnected from NotificationHub", userId, role);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "User {UserId} disconnected with error", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Sends a notification to a specific user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notification">Notification object</param>
    public async Task SendNotificationToUser(string userId, object notification)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        _logger.LogInformation("Notification sent to user {UserId}", userId);
    }

    /// <summary>
    /// Sends a notification to all users with a specific role
    /// </summary>
    /// <param name="role">Target role</param>
    /// <param name="notification">Notification object</param>
    public async Task SendNotificationToRole(string role, object notification)
    {
        await Clients.Group(role).SendAsync("ReceiveNotification", notification);
        _logger.LogInformation("Notification sent to role {Role}", role);
    }

    /// <summary>
    /// Sends a notification to all connected clients
    /// </summary>
    /// <param name="notification">Notification object</param>
    public async Task SendNotificationToAll(object notification)
    {
        await Clients.All.SendAsync("ReceiveNotification", notification);
        _logger.LogInformation("Notification sent to all users");
    }
}
