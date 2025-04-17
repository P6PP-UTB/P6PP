using System;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Persistence.Entities.DB;
using src.NotificationService.API.Persistence.Entities.DB.Models;

namespace src.NotificationService.API.Services;

public class NotificationLogService
{
    private readonly NotificationDbContext _notificationDbContext;

    public NotificationLogService(NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    /// <summary>
    /// Creates new notification log
    /// </summary>
    public async Task LogNotification(int userId, NotificationType type, string subject, string text)
    {
        await _notificationDbContext.NotificationLogs.AddAsync(
            new NotificationLog
            {
                UserId = userId,
                NotificationType = type,
                Subject = subject,
                Text = text,
                SentDate = DateTime.Now,
                HasBeeenRead = false,
            }
        );
        await _notificationDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get all notifications for <param>userId</userId>
    /// If <param>unreadOnly</param> is true, only unread notifications are returned
    /// Only unread notifications are sent by default
    /// </summary>
    public async Task<List<NotificationLog>> GetNotificationsFor(int userId, bool unreadOnly = true)
    {
        var logs =  _notificationDbContext.NotificationLogs
            .Where(n => (n.UserId == userId));
        if (unreadOnly) {
            logs = logs.Where(n => n.HasBeeenRead == false);
        }
        return await logs.ToListAsync();
    }

    public async Task<int> SetAllNotificationsAsRead(int userId)
    {
        var notificationsChanged = await _notificationDbContext
            .NotificationLogs
            .Where(n => n.UserId == userId)
            .ExecuteUpdateAsync(n => n.SetProperty(x => x.HasBeeenRead, true));

        await _notificationDbContext.SaveChangesAsync();
        return notificationsChanged;
    }

    public async Task<int> SetSomeNotificationsAsRead(List<int> notificationIds)
    {
        var notificationsChanged = await _notificationDbContext
            .NotificationLogs
            .Where(n => notificationIds.Contains(n.Id))
            .ExecuteUpdateAsync(n => n.SetProperty(x => x.HasBeeenRead, true));

        await _notificationDbContext.SaveChangesAsync();
        return notificationsChanged;
    }
}
