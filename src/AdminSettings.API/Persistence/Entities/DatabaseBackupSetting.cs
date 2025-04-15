using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using AdminSettings.Persistence.Enums;

namespace AdminSettings.Persistence.Entities;

public class DatabaseBackupSetting
{
    public int Id { get; set; }
    public bool ManualBackupEnabled { get; set; } = true;
    public bool AutomaticBackupEnabled { get; set; } = true;
    [Column(TypeName = "int")]
    public BackupFrequency BackupFrequency { get; set; } = BackupFrequency.Monthly;
    public TimeOnly BackupTime { get; set; } = new TimeOnly(0, 0);
}