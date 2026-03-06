using System.Reflection;
using DellinTerminalImporter.Entities;
using Microsoft.EntityFrameworkCore;

namespace DellinTerminalImporter.Data;

public class DellinDictionaryDbContext : DbContext
{
    public DbSet<OfficeEntity> Offices { get; set; }
    public DbSet<PhoneEntity> Phones { get; set; }

    public DellinDictionaryDbContext(DbContextOptions<DellinDictionaryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
