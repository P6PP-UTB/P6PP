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
                SentDate = DateTime.Now
            }
        );
        await _notificationDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get all notifications for <param>userId</userId>
    /// </summary>
    public async Task<List<NotificationLog>> GetNotificationsFor(int userId)
    {
        // TODO: Some sort of paging or result limiting should be probably implemented
        var logs = await _notificationDbContext.NotificationLogs
            .Where(n => n.UserId == userId)
            .ToListAsync();

        return logs;
    }
}
