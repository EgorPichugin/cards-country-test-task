using DellinTerminalImporter.Data;
using DellinTerminalImporter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<DellinDictionaryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHostedService<TerminalImportService>();
        return services;
    }
}