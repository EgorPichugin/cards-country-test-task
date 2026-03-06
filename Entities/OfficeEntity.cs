namespace DellinTerminalImporter.Entities;

public class OfficeEntity
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int CityCode { get; set; }
    public string? Uuid { get; set; }
    public OfficeTypeEntity? Type { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public CoordinatesEntity Coordinates { get; set; } = new();
    public AddressEntity Address { get; set; } = new();
    public string WorkTime { get; set; } = string.Empty;
    public List<PhoneEntity> Phones { get; set; } = new();

    public OfficeEntity() { }
}
