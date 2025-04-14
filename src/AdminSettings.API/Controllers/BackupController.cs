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
        var success = await _backupService.BackupAllAsync();

        if (success)
        {
            return Ok("✅ Zálohování dokončeno.");
        }
        else
        {
            return BadRequest("❌ Zálohování nebylo provedeno.");
        }
    }
}