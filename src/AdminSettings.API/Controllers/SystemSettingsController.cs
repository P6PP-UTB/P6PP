using Microsoft.AspNetCore.Mvc;
using AdminSettings.Services;
using System.Threading.Tasks;
using AdminSettings.Persistence.Entities;

namespace AdminSettings.Controllers
{
    [ApiController]
    [Route("api")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly SystemSettingsService _systemSettingsService;

        public SystemSettingsController(SystemSettingsService systemSettingsService)
        {
            _systemSettingsService = systemSettingsService;
        }

        [HttpGet("system-settings")]
        public async Task<IActionResult> GetSystemSettings()
        {
            var settings = await _systemSettingsService.GetSystemSettingsAsync();
            return settings == null ? NotFound("System settings not found.") : Ok(settings);
        }

        [HttpPut("system-settings")]
        public async Task<IActionResult> UpdateSystemSettings([FromBody] SystemSetting settings)
        {
            if (!ModelState.IsValid || settings == null)
                return BadRequest("Invalid data.");

            var result = await _systemSettingsService.UpdateSystemSettingsAsync(settings);
            return result ? NoContent() : NotFound("System settings not found.");
        }

        [HttpGet("system-settings/BackupSetting")]
        public async Task<IActionResult> GetDatabaseBackupSetting()
        {
            var databaseBackupSetting = await _systemSettingsService.GetDatabaseBackupSettingsAsync();
            return Ok(databaseBackupSetting);
        }

        [HttpPut("system-settings/BackupSetting")]
        public async Task<IActionResult> UpdateDatabaseBackupSetting([FromBody] DatabaseBackupSetting backupSetting)
        {
            if (!ModelState.IsValid || backupSetting == null)
                return BadRequest("Invalid backup setting data.");
            var result = await _systemSettingsService.UpdateDatabaseBackupSettingAsync(backupSetting);
            return result ? NoContent() : NotFound("Backup setting not found.");
        }

        [HttpGet("system-settings/audit-log-enabled")]
        public async Task<IActionResult> GetAuditLogEnabled()
        {
            var auditLogEnabled = await _systemSettingsService.GetAuditLogEnabledAsync();
            return Ok(auditLogEnabled);
        }

        [HttpPut("system-settings/audit-log-enabled/{enabled}")]
        public async Task<IActionResult> SetAuditLogEnabled(bool enabled)
        {
            var settings = await _systemSettingsService.GetSystemSettingsAsync();
            if (settings == null)
                return NotFound(new { Message = "System settings not found." });

            settings.AuditLogEnabled = enabled;
            var result = await _systemSettingsService.UpdateSystemSettingsAsync(settings);
            return result
                ? Ok(new { Message = $"Audit log has been {(enabled ? "enabled" : "disabled")}." })
                : StatusCode(500, new { Message = "Failed to update audit log setting." });
        }

        [HttpGet("system-settings/notification-enabled")]
        public async Task<IActionResult> GetNotificationEnabled()
        {
            var notificationEnabled = await _systemSettingsService.GetNotificationEnabledAsync();
            return Ok(notificationEnabled);
        }

        [HttpPut("system-settings/backup/manual/{enabled}")]
        public async Task<IActionResult> SetManualBackupEnabled(bool enabled)
        {
            var settings = await _systemSettingsService.GetSystemSettingsAsync();
            if (settings == null)
                return NotFound(new { Message = "System settings not found." });

            settings.DatabaseBackupSetting.ManualBackupEnabled = enabled;
            var result = await _systemSettingsService.UpdateSystemSettingsAsync(settings);
            return result
                ? Ok(new { Message = $"Manual backup has been {(enabled ? "enabled" : "disabled")}." })
                : StatusCode(500, new { Message = "Failed to update manual backup setting." });
        }

        [HttpPut("system-settings/backup/automatic/{enabled}")]
        public async Task<IActionResult> SetAutomaticBackupEnabled(bool enabled)
        {
            var settings = await _systemSettingsService.GetSystemSettingsAsync();
            if (settings == null)
                return NotFound(new { Message = "System settings not found." });

            settings.DatabaseBackupSetting.AutomaticBackupEnabled = enabled;
            var result = await _systemSettingsService.UpdateSystemSettingsAsync(settings);
            return result
                ? Ok(new { Message = $"Automatic backup has been {(enabled ? "enabled" : "disabled")}." })
                : StatusCode(500, new { Message = "Failed to update automatic backup setting." });
        }
    }
}
