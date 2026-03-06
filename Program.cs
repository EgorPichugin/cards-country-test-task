using DellinTerminalImporter.Data;
using DellinTerminalImporter.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddDbContext<DellinDictionaryDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddSingleton<TerminalJsonMapper>();
        builder.Services.AddHostedService<TerminalImportService>();
        var host = builder.Build();

        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();
            await db.Database.MigrateAsync();
        }

        await host.RunAsync();
    }
}
