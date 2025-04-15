using Microsoft.AspNetCore.Mvc;
using AdminSettings.Services;

namespace AdminSettings.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackupController : ControllerBase
{
    private readonly DatabaseBackupService _backupService;

    public BackupController(DatabaseBackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunBackup()

    {
        var systemSettings = await _backupService.GetSystemSettingsAsync();
        if (systemSettings?.DatabaseBackupSetting == null)
        {
            return StatusCode(500, "❌ System settings not found. Backup cannot be performed.");
        }

        // check if Manual backup is enabled
        if (!systemSettings.DatabaseBackupSetting.ManualBackupEnabled)
        {
            return BadRequest("❌ Manual backup is disabled. Please enable it to perform a backup.");
        }

        var success = await _backupService.BackupAllAsync();

        if (success)
        {
            return Ok("✅ Backup completed.");
        }
        else
        {
            return StatusCode(500, "❌ Backup not performed due to a server error.");
        }
    }
}