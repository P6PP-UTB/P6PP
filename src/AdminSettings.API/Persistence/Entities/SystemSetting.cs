using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdminSettings.Persistence.Entities;

public class SystemSetting
{
    public int Id { get; set; }

    [ForeignKey("Timezone")]
    public int TimezoneId { get; set; }
    public Timezone Timezone { get; set; }

    public string SystemLanguage { get; } = "en-US";

    public bool AuditLogEnabled { get; set; } = true;

    [ForeignKey("DatabaseBackupSetting")]
    public int DatabaseBackupSettingId { get; set; }
    public DatabaseBackupSetting DatabaseBackupSetting { get; set; }

    public bool NotificationEnabled { get; set; } = true;
}



