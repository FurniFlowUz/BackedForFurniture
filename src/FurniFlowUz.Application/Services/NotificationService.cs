using AutoMapper;
using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for notification management
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, CancellationToken cancellationToken = default)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Get notifications for this specific user or for their role
        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => (n.UserId == userId || (n.UserId == null && n.Role == user.Role)),
            cancellationToken);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications.OrderByDescending(n => n.CreatedAt));
    }

    public async Task<IEnumerable<NotificationDto>> GetRoleNotificationsAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => n.Role == role,
            cancellationToken);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications.OrderByDescending(n => n.CreatedAt));
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto request, CancellationToken cancellationToken = default)
    {
        // If target user is specified, validate user exists
        if (request.UserId.HasValue)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId.Value, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.UserId.Value);
            }
        }

        // Create notification
        var notification = new Notification
        {
            Title = request.Title,
            Message = request.Message,
            Type = Enum.Parse<NotificationType>(request.Type),
            UserId = request.UserId,
            Role = !string.IsNullOrEmpty(request.Role) ? Enum.Parse<UserRole>(request.Role) : null,
            IsRead = false,
            RelatedEntityType = request.RelatedEntityType,
            RelatedEntityId = request.RelatedEntityId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Integrate with SignalR hub for real-time notifications
        // This will be implemented in the API layer

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
        {
            throw new NotFoundException(nameof(Notification), notificationId);
        }

        notification.IsRead = true;

        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Get all unread notifications for this user
        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => !n.IsRead && (n.UserId == userId || (n.UserId == null && n.Role == user.Role)),
            cancellationToken);

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteNotificationAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
        {
            throw new NotFoundException(nameof(Notification), notificationId);
        }

        _unitOfWork.Notifications.Remove(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
