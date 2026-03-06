namespace DellinTerminalImporter.Entities;

public class PhoneEntity
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string? PhoneType { get; set; }
    public string? Comment { get; set; }
    public bool Primary { get; set; }
    public int OfficeId { get; set; }
}
