namespace DellinTerminalImporter.Json;

public class TerminalsRoot
{
    public List<CityDto> City { get; set; } = new();
}

public class CityDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int? CityId { get; set; }

    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? RequestEndTime { get; set; }
    public TerminalsWrapper? Terminals { get; set; }
}

public class TerminalsWrapper
{
    public List<TerminalDto> Terminal { get; set; } = new();
}

public class TerminalDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? FullAddress { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public bool IsPvz { get; set; }
    public bool IsOffice { get; set; }
    public string? Mail { get; set; }
    public List<PhoneDto> Phones { get; set; } = new();
}

public class PhoneDto
{
    public string Number { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Comment { get; set; }
    public bool Primary { get; set; }
}
