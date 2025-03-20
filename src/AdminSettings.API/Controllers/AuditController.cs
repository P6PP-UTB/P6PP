using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using AdminSettings.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminSettings.Controllers;

[Route("api/audit")]
[ApiController]
public class AuditController : ControllerBase
{
    private readonly AdminSettingsDbContext _context;
    private readonly UserService _userService;

    public AuditController(AdminSettingsDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs()
    {
        var auditLogs = await _context.AuditLogs.ToListAsync();
        return Ok(auditLogs);
    }

    [HttpPost]
    public async Task<IActionResult> AddAuditLog([FromBody] AuditLog auditLog)
    {
        if (auditLog == null || string.IsNullOrEmpty(auditLog.UserId) || string.IsNullOrEmpty(auditLog.Action))
            return BadRequest("Invalid audit log data");

        await _context.AuditLogs.AddAsync(auditLog);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAuditLogs), new { id = auditLog.Id }, auditLog);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAuditLogs(string userId)
    {
        var user = await _userService.GetUserById(userId);
        if (user == null)
            return NotFound("User not found");

        var auditLogs = await _context.AuditLogs
            .Where(a => a.UserId == user.Id)
            .ToListAsync();

        return Ok(new { User = user, AuditLogs = auditLogs });
    }

    [HttpPut("{id}/archive")]
    public async Task<IActionResult> ArchiveAuditLog(int id)
    {
        var auditLog = await _context.AuditLogs.FindAsync(id);
        if (auditLog == null)
            return NotFound();

        auditLog.Action = "Archived";
        _context.Entry(auditLog).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(auditLog);
    }

    [HttpGet("action/{action}")]
    public async Task<IActionResult> GetAuditLogsByAction(string action)
    {
        var auditLogs = await _context.AuditLogs.Where(a => a.Action == action).ToListAsync();
        return Ok(auditLogs);
    }
}
