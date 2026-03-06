using System.Globalization;
using DellinTerminalImporter.Entities;
using DellinTerminalImporter.Json;

namespace DellinTerminalImporter.Services;

public class TerminalJsonMapper(ILogger<TerminalJsonMapper> logger)
{
    public List<OfficeEntity> Map(TerminalsRoot root)
    {
        var offices = new List<OfficeEntity>();
        var seenIds = new HashSet<int>();

        foreach (var city in root.City)
        {
            var terminals = city.Terminals?.Terminal;
            if (terminals is null || terminals.Count == 0)
                continue;

            foreach (var terminal in terminals)
            {
                if (!TryParseId(terminal.Id, out var id))
                    continue;

                if (!seenIds.Add(id))
                {
                    logger.LogWarning("Skipped duplicate terminal id={Id}", id);
                    continue;
                }

                offices.Add(MapTerminal(city, terminal, id));
            }
        }

        return offices;
    }

    private bool TryParseId(string raw, out int id)
    {
        if (int.TryParse(raw, out id))
            return true;

        logger.LogWarning("Skipped terminal with invalid id: '{TerminalId}'", raw);
        return false;
    }

    private static OfficeEntity MapTerminal(CityDto city, TerminalDto terminal, int id)
    {
        double.TryParse(terminal.Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat);
        double.TryParse(terminal.Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lon);

        return new OfficeEntity
        {
            Id = id,
            Code = city.Code,
            CityCode = city.CityId ?? 0,
            CountryCode = "RU",
            Type = ResolveOfficeType(terminal),
            Coordinates = new CoordinatesEntity { Latitude = lat, Longitude = lon },
            Address = new AddressEntity
            {
                ShortAddress = terminal.Address,
                FullAddress = terminal.FullAddress
            },
            WorkTime = city.RequestEndTime ?? string.Empty,
            Phones = terminal.Phones
                .Select(p => new PhoneEntity
                {
                    Number = p.Number,
                    PhoneType = p.Type,
                    Comment = p.Comment,
                    Primary = p.Primary
                })
                .ToList()
        };
    }

    private static OfficeTypeEntity ResolveOfficeType(TerminalDto terminal) =>
        terminal.IsPvz ? OfficeTypeEntity.Pvz :
        terminal.IsOffice ? OfficeTypeEntity.Office :
        OfficeTypeEntity.Terminal;
}
