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
            var databaseBackupSetting = await _context.DatabaseBackupSettings.FirstAsync();

            _context.SystemSettings.Add(new SystemSetting
            {
                DatabaseBackupSettingId = databaseBackupSetting.Id,
                DatabaseBackupSetting = databaseBackupSetting
            });

            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Default system settings data seeded.");
    }
}