using System.Text.Json;
using DellinTerminalImporter.Data;
using DellinTerminalImporter.Json;
using DellinTerminalImporter.Entities;
using Microsoft.EntityFrameworkCore;

namespace DellinTerminalImporter.Services;

public class TerminalImportService(
    IServiceScopeFactory scopeFactory,
    ILogger<TerminalImportService> logger,
    IConfiguration configuration,
    TerminalJsonMapper mapper) : BackgroundService
{
    private static readonly TimeZoneInfo MskTimeZone = ResolveMskTimeZone();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Service started. Initial import will run at the next scheduled time (default 02:00 MSK).");

        while (!cancellationToken.IsCancellationRequested)
        {
            var delay = CalculateDelayUntilNextRun();
            logger.LogInformation(
                "Next import scheduled in {Delay:hh\\:mm\\:ss} (at 02:00 MSK)",
                delay);

            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Service stopped => exit loop
                break;
            }

            try
            {
                await ImportAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error importing terminals: {Exception}", exception.Message);
            }
        }

        logger.LogInformation("Service stopped.");
    }

    private async Task ImportAsync(CancellationToken cancellationToken)
    {
        var relativePath = configuration["TerminalImport:JsonFilePath"] ?? "files/terminals.json";
        var jsonFilePath = Path.GetFullPath(relativePath);

        logger.LogInformation("Starting import from file {FilePath}", jsonFilePath);

        if (!File.Exists(jsonFilePath))
        {
            logger.LogError("File not found: {FilePath}", jsonFilePath);
            return;
        }

        var root = await DeserializeJsonAsync(jsonFilePath, cancellationToken);
        if (root?.City is null || root.City.Count == 0)
        {
            logger.LogWarning("JSON does not contain city data or is empty.");
            return;
        }

        var offices = mapper.Map(root);
        logger.LogInformation("Loaded {Count} terminals from JSON", offices.Count);

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Count and remove existing data
            // Phones are deleted automatically via ON DELETE CASCADE
            var oldCount = await context.Offices.CountAsync(cancellationToken);
            await context.Offices.ExecuteDeleteAsync(cancellationToken);
            logger.LogInformation("Deleted {OldCount} old records", oldCount);

            // Bulk insert in batches to limit memory pressure
            const int batchSize = 500;
            var saved = 0;

            for (var i = 0; i < offices.Count; i += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = offices.GetRange(i, Math.Min(batchSize, offices.Count - i));
                await context.Offices.AddRangeAsync(batch, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                context.ChangeTracker.Clear();
                saved += batch.Count;
            }

            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("Saved {NewCount} new terminals", saved);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    private async Task<TerminalsRoot?> DeserializeJsonAsync(
        string filePath, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        return await JsonSerializer.DeserializeAsync<TerminalsRoot>(stream, options, cancellationToken);
    }

    private TimeSpan CalculateDelayUntilNextRun()
    {
        var scheduleHour = configuration.GetValue("TerminalImport:ScheduleHour", 2);
        var scheduleMinute = configuration.GetValue("TerminalImport:ScheduleMinute", 0);

        var currentMskTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, MskTimeZone);
        var nextRun = currentMskTime.Date.AddHours(scheduleHour).AddMinutes(scheduleMinute);

        if (currentMskTime >= nextRun)
            nextRun = nextRun.AddDays(1);

        return nextRun - currentMskTime;
    }

    private static TimeZoneInfo ResolveMskTimeZone()
    {
        // Windows uses "Russian Standard Time"; Linux/macOS use IANA "Europe/Moscow"
        if (TimeZoneInfo.TryFindSystemTimeZoneById("Russian Standard Time", out var timeZone))
            return timeZone;
        if (TimeZoneInfo.TryFindSystemTimeZoneById("Europe/Moscow", out timeZone))
            return timeZone;

        // Fallback: UTC+3 fixed offset
        return TimeZoneInfo.CreateCustomTimeZone(
            "MSK", TimeSpan.FromHours(3),
            "Moscow Standard Time", "Moscow Standard Time");
    }
}
