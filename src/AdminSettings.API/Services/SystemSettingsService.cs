using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdminSettings.Services
{
    public class SystemSettingsService
    {
        private readonly AdminSettingsDbContext _context;

        public SystemSettingsService(AdminSettingsDbContext context)
        {
            _context = context;
        }

        public async Task<SystemSetting?> GetSystemSettingsAsync()
        {
            return await _context.SystemSettings
                .Include(s => s.Timezone)
                .Include(s => s.DatabaseBackupSetting)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateSystemSettingsAsync(SystemSetting settings)
        {
            var existingSettings = await _context.SystemSettings.FirstOrDefaultAsync();

            if (existingSettings == null)
                return false;

            existingSettings.TimezoneId = settings.TimezoneId;
            existingSettings.AuditLogEnabled = settings.AuditLogEnabled;
            existingSettings.NotificationEnabled = settings.NotificationEnabled;
            existingSettings.DatabaseBackupSettingId = settings.DatabaseBackupSettingId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Timezone>> GetTimezonesAsync()
        {
            return await _context.Timezones.ToListAsync();
        }

        public async Task<bool> UpdateTimezoneAsync(Timezone timezone)
        {
            var existingTimezone = await _context.Timezones.FirstOrDefaultAsync(t => t.Id == timezone.Id);

            if (existingTimezone == null)
                return false;

            existingTimezone.Name = timezone.Name;
            existingTimezone.UtcOffset = timezone.UtcOffset;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GetAuditLogEnabledAsync()
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            return settings?.AuditLogEnabled ?? false;
        }

        public async Task<bool> GetNotificationEnabledAsync()
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            return settings?.NotificationEnabled ?? false;
        }

        public async Task<List<DatabaseBackupSetting>> GetDatabaseBackupSettingsAsync()
        {
            return await _context.DatabaseBackupSettings.ToListAsync();
        }

        public async Task<bool> UpdateDatabaseBackupSettingAsync(DatabaseBackupSetting backupSetting)
        {
            var existingBackupSetting = await _context.DatabaseBackupSettings.FirstOrDefaultAsync(b => b.Id == backupSetting.Id);

            if (existingBackupSetting == null)
                return false;

            existingBackupSetting.ManualBackupEnabled = backupSetting.ManualBackupEnabled;
            existingBackupSetting.AutomaticBackupEnabled = backupSetting.AutomaticBackupEnabled;
            existingBackupSetting.BackupFrequency = backupSetting.BackupFrequency;
            existingBackupSetting.BackupTime = backupSetting.BackupTime;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
