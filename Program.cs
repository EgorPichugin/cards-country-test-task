using DellinTerminalImporter.Data;
using DellinTerminalImporter.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddServices(builder.Configuration);
        var host = builder.Build();

        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();
            await db.Database.MigrateAsync();
        }

        await host.RunAsync();
    }
}
