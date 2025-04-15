namespace AdminSettings.Persistence.Entities;

public class Timezone
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string UtcOffset { get; set; }
}
