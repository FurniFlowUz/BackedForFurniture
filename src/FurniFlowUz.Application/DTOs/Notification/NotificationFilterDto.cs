using FurniFlowUz.Application.DTOs.Common;

namespace FurniFlowUz.Application.DTOs.Notification;

/// <summary>
/// Filter DTO for notification queries
/// </summary>
public class NotificationFilterDto : BaseFilter
{
    /// <summary>
    /// Filter by read status
    /// </summary>
    public bool? IsRead { get; set; }

    /// <summary>
    /// Filter by notification type
    /// </summary>
    public string? Type { get; set; }
}
