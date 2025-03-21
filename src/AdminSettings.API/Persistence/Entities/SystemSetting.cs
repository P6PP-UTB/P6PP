using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdminSettings.Persistence.Entities;

public class SystemSetting
{
    public int Id { get; set; }

    [ForeignKey("Timezone")]
    public int TimezoneId { get; set; }
    public Timezone timezone { get; set; }

    [ForeignKey("Currency")]
    public int CurrencyId { get; set; }
    public Currency currency { get; set; }

    [ForeignKey("Language")]
    public int LanguageId { get; set; }
    public Language language { get; set; }
}



