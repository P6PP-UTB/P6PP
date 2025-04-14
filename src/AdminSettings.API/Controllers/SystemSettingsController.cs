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

        [HttpGet("system-settings/timezones")]
        public async Task<IActionResult> GetTimezones()
        {
            var timezones = await _systemSettingsService.GetTimezonesAsync();
            return timezones.Count > 0 ? Ok(timezones) : NotFound("No timezones available.");
        }

        [HttpPut("system-settings/timezones")]
        public async Task<IActionResult> UpdateTimezone([FromBody] Timezone timezone)
        {
            if (!ModelState.IsValid || timezone == null)
                return BadRequest("Invalid timezone data.");

            var result = await _systemSettingsService.UpdateTimezoneAsync(timezone);
            return result ? NoContent() : NotFound("Timezone not found.");
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

        [HttpGet("system-settings/notification-enabled")]
        public async Task<IActionResult> GetNotificationEnabled()
        {
            var notificationEnabled = await _systemSettingsService.GetNotificationEnabledAsync();
            return Ok(notificationEnabled);
        }
    }
}
