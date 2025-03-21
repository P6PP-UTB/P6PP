using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using AdminSettings.Services;

namespace AdminSettings.Controllers;

[ApiController]
[Route("api")]
public class SystemSettingsController : ControllerBase
{
    private readonly AdminSettingsDbContext _context;

    public SystemSettingsController(AdminSettingsDbContext context)
    {
        _context = context;
    }

    [HttpGet("system-settings")]
    public async Task<IActionResult> GetSystemSettings()
    {
        var settings = await _context.SystemSettings
            .Include(s => s.timezone) 
            .Include(s => s.currency) 
            .FirstOrDefaultAsync();
        return settings == null ? NotFound("System settings not found.") : Ok(settings);
    }

    [HttpPut("system-settings")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSystemSettings([FromBody] SystemSetting settings)
    {
        if (!ModelState.IsValid || settings == null)
            return BadRequest("Invalid data.");

        var existingSettings = await _context.SystemSettings.FirstOrDefaultAsync();
        if (existingSettings == null)
            return NotFound("System settings not found.");

        existingSettings.TimezoneId = settings.TimezoneId;
        existingSettings.CurrencyId = settings.CurrencyId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("system-settings/timezones")]
    public async Task<IActionResult> GetTimezones()
    {
        var timezones = await _context.Timezones.ToListAsync();
        return timezones.Any() ? Ok(timezones) : NotFound("No timezones available.");
    }

    [HttpPut("system-settings/timezones")]
    public async Task<IActionResult> UpdateTimezone([FromBody] Timezone timezone)
    {
        if (!ModelState.IsValid || timezone == null)
            return BadRequest("Invalid timezone data.");

        var existingTimezone = await _context.Timezones.FirstOrDefaultAsync(t => t.Id == timezone.Id);
        if (existingTimezone == null)
            return NotFound("Timezone not found.");

        existingTimezone.Name = timezone.Name;
        existingTimezone.UtcOffset = timezone.UtcOffset;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("system-settings/currencies")]
    public async Task<IActionResult> GetCurrencies()
    {
        var currencies = await _context.Currencies.ToListAsync();
        return currencies.Any() ? Ok(currencies) : NotFound("No currencies available.");
    }

    [HttpPut("system-settings/currencies")]
    public async Task<IActionResult> UpdateCurrency([FromBody] Currency currency)
    {
        if (!ModelState.IsValid || currency == null)
            return BadRequest("Invalid currency data.");

        var existingCurrency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == currency.Id);
        if (existingCurrency == null)
            return NotFound("Currency not found.");

        existingCurrency.Name = currency.Name;
        existingCurrency.Symbol = currency.Symbol;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("system-settings/languages")]
    public async Task<IActionResult> GetLanguages()
    {
        var languages = await _context.Languages.ToListAsync();
        return languages.Any() ? Ok(languages) : NotFound("No languages available.");
    }

    [HttpPut("system-settings/languages")]
    public async Task<IActionResult> UpdateLanguage([FromBody] Language language)
    {
        if (!ModelState.IsValid || language == null)
            return BadRequest("Invalid language data.");

        var existingLanguage = await _context.Languages.FirstOrDefaultAsync(l => l.Id == language.Id);
        if (existingLanguage == null)
            return NotFound("Language not found.");

        existingLanguage.Locale = language.Locale;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}