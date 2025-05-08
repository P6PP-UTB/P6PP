using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdminSettings.Persistence.Entities;

public class SystemSetting
{
    public int Id { get; set; }

    public string SystemLanguage { get; } = "en-US";

    public bool AuditLogEnabled { get; set; } = true;

    [ForeignKey("DatabaseBackupSetting")]
    public int DatabaseBackupSettingId { get; set; }
    public required DatabaseBackupSetting DatabaseBackupSetting { get; set; }

    public bool NotificationEnabled { get; set; } = true;
}



