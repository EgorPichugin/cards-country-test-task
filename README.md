# Dellin Terminal Importer

Background service (.NET 9) that imports terminal data from JSON into PostgreSQL daily at 02:00 MSK.

## Quick start

```bash
# 1. Start PostgreSQL
docker compose up -d

# 2. Place source file
cp /path/to/terminals.json files/terminals.json

# 3. Create migration (first time only)
dotnet ef migrations add InitialCreate --project DellinTerminalImporter.csproj

# 4. Run (migrations applied automatically on startup)
dotnet run --project DellinTerminalImporter.csproj
```

## Configuration — `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=dellin_dictionary;Username=postgres;Password=postgres"
},
"TerminalImport": {
  "JsonFilePath": "files/terminals.json",
  "ScheduleHour": 2,
  "ScheduleMinute": 0
}
```

## Test without waiting for 02:00

Set `ScheduleHour`/`ScheduleMinute` to current Moscow time + a few minutes, restart.
