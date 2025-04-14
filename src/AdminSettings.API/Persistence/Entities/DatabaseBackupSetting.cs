using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AdminSettings.Persistence.Entities;

public class DatabaseBackupSetting
{
    public int Id { get; set; }
    public bool BackupEnabled { get; set; } = true;
    public string BackupFrequency { get; set; } = "monthly";
    public TimeOnly BackupTime { get; set; } = new TimeOnly(0, 0);
}