using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace AdminSettings.Data;

public class SystemSettingsSeeder
{
    private readonly AdminSettingsDbContext _context;
    private readonly ILogger<SystemSettingsSeeder> _logger;

    public SystemSettingsSeeder(AdminSettingsDbContext context, ILogger<SystemSettingsSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Seeding default data for system settings...");

        // Seed Timezone
        if (!await _context.Timezones.AnyAsync())
        {
            _context.Timezones.AddRange(
                new Timezone { Name = "UTC", UtcOffset = "+00:00" },
                new Timezone { Name = "CET", UtcOffset = "+01:00" },
                new Timezone { Name = "CEST", UtcOffset = "+02:00" }
            );
        }

        // Seed DatabaseBackupSetting
        if (!await _context.DatabaseBackupSettings.AnyAsync())
        {
            _context.DatabaseBackupSettings.Add(new DatabaseBackupSetting
            {
                ManualBackupEnabled = true,
                BackupFrequency = BackupFrequency.Monthly,
                BackupTime = new TimeOnly(0, 0)
            });

            await _context.SaveChangesAsync();
        }

        await _context.SaveChangesAsync();

        // Seed SystemSettings
        if (!await _context.SystemSettings.AnyAsync())
        {
            var timezone = await _context.Timezones.FirstAsync();
            var databaseBackupSetting = await _context.DatabaseBackupSettings.FirstAsync();

            _context.SystemSettings.Add(new SystemSetting
            {
                TimezoneId = timezone.Id,
                DatabaseBackupSettingId = databaseBackupSetting.Id,
                Timezone = timezone,
                DatabaseBackupSetting = databaseBackupSetting
            });

            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Default system settings data seeded.");
    }
}