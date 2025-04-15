
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AdminSettings.Services;
using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Enums;

namespace AdminSettings.Services
{
    public class BackupSchedulerService : BackgroundService
    {
        private readonly ILogger<BackupSchedulerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackupSchedulerService(ILogger<BackupSchedulerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var systemSettingsService = scope.ServiceProvider.GetRequiredService<SystemSettingsService>();
                        var backupService = scope.ServiceProvider.GetRequiredService<DatabaseBackupService>();

                        var systemSettings = await systemSettingsService.GetSystemSettingsAsync();

                        if (systemSettings?.DatabaseBackupSetting != null && systemSettings.DatabaseBackupSetting.AutomaticBackupEnabled)
                        {
                            var backupSetting = systemSettings.DatabaseBackupSetting;

                            if (ShouldRunBackup(backupSetting))
                            {
                                _logger.LogInformation("Running a scheduled database backup.");
                                await backupService.BackupAllAsync();
                                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while scheduling backups.");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private bool ShouldRunBackup(DatabaseBackupSetting settings)
        {
            DateTime now = DateTime.Now;
            TimeOnly currentTime = TimeOnly.FromDateTime(now);

            bool isTimeToBackup = Math.Abs((currentTime.ToTimeSpan() - settings.BackupTime.ToTimeSpan()).TotalMinutes) < 1;

            if (!isTimeToBackup)
                return false;

            return settings.BackupFrequency switch
            {
                BackupFrequency.Daily => true,
                BackupFrequency.Weekly => now.DayOfWeek == DayOfWeek.Sunday,
                BackupFrequency.Monthly => now.Day == 1,
                _ => false
            };
        }
    }
}
