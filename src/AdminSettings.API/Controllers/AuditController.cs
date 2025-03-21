using AdminSettings.Persistence.Entities;
using AdminSettings.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminSettings.Controllers;

[Route("api/audit")]
[ApiController]
public class AuditController : ControllerBase
{
    private readonly AuditLogService _auditLogService;
    private readonly UserService _userService;

    public AuditController(AuditLogService auditLogService, UserService userService)
    {
        _auditLogService = auditLogService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs()
    {
        var logs = await _auditLogService.GetAllAsync();
        return Ok(logs);
    }

    [HttpPost]
    public async Task<IActionResult> AddAuditLog([FromBody] AuditLog auditLog)
    {
        if (auditLog == null || string.IsNullOrEmpty(auditLog.UserId) || string.IsNullOrEmpty(auditLog.Action))
            return BadRequest("Invalid data");

        var id = await _auditLogService.CreateAsync(auditLog);
        return CreatedAtAction(nameof(GetAuditLogs), new { id }, auditLog);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAuditLogs(string userId)
    {
        var user = await _userService.GetUserById(userId);
        if (user == null)
            return NotFound("User not found");

        var logs = await _auditLogService.GetByUserAsync(userId);
        return Ok(new { User = user, AuditLogs = logs });
    }

    [HttpGet("action/{action}")]
    public async Task<IActionResult> GetAuditLogsByAction(string action)
    {
        var logs = await _auditLogService.GetByActionAsync(action);
        return Ok(logs);
    }

    [HttpPut("{id}/archive")]
    public async Task<IActionResult> ArchiveAuditLog(int id)
    {
        await _auditLogService.ArchiveAsync(id);
        return Ok();
    }
}
