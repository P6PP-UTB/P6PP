using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemService.API.Data;
using System.Linq;
using System.Threading.Tasks;
using SystemService.API.Persistence.Entities;

namespace SystemManagement.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class SystemController : ControllerBase
    {
        private readonly SystemDbContext _context;

        public SystemController(SystemDbContext context)
        {
            _context = context;
        }

        [HttpGet("system-settings")]
        public async Task<IActionResult> GetSystemSettings()
        {
            var settings = await _context.SystemSettings
                .Include(s => s.timezone) 
                .Include(s => s.currency) 
                .Include(s => s.openingHours) 
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
            existingSettings.TaxRate = settings.TaxRate;
            existingSettings.OpeningHoursId = settings.OpeningHoursId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("timezones")]
        public async Task<IActionResult> GetTimezones()
        {
            var timezones = await _context.Timezones.ToListAsync();
            return timezones.Any() ? Ok(timezones) : NotFound("No timezones available.");
        }

        [HttpPut("timezones")]
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

        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _context.Currencies.ToListAsync();
            return currencies.Any() ? Ok(currencies) : NotFound("No currencies available.");
        }

        [HttpPut("currencies")]
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

        [HttpPut("tax-rate")]
        public async Task<IActionResult> UpdateTaxRate([FromBody] decimal taxRate)
        {
            if (taxRate < 0)
                return BadRequest("Tax rate cannot be negative.");

            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null)
                return NotFound("System settings not found.");

            settings.TaxRate = taxRate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("opening-hours")]
        public async Task<IActionResult> GetOpeningHours()
        {
            var hours = await _context.OpeningHours.FirstOrDefaultAsync();
            return hours == null ? NotFound("Opening hours not found.") : Ok(hours);
        }

        [HttpPut("opening-hours")]
        public async Task<IActionResult> UpdateOpeningHours([FromBody] OpeningHours openingHours)
        {
            if (!ModelState.IsValid || openingHours == null)
                return BadRequest("Invalid opening hours data.");

            var existingOpeningHours = await _context.OpeningHours.FirstOrDefaultAsync(o => o.Id == openingHours.Id);
            if (existingOpeningHours == null)
                return NotFound("Opening hours not found.");

            existingOpeningHours.DayOfWeek = openingHours.DayOfWeek;
            existingOpeningHours.OpenTime = openingHours.OpenTime;
            existingOpeningHours.CloseTime = openingHours.CloseTime;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Zbytek je z ostatních databází
    }
}