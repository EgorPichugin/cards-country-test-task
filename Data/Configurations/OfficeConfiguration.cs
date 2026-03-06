using DellinTerminalImporter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DellinTerminalImporter.Data.Configurations;

public class OfficeConfiguration : IEntityTypeConfiguration<OfficeEntity>
{
    public void Configure(EntityTypeBuilder<OfficeEntity> builder)
    {
        builder.ToTable("offices");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.Code).HasMaxLength(50);
        builder.Property(o => o.CountryCode).HasMaxLength(10).IsRequired();
        builder.Property(o => o.Uuid).HasMaxLength(36);
        builder.Property(o => o.WorkTime).HasMaxLength(200);
        builder.Property(o => o.Type).HasConversion<int>();

        builder.OwnsOne<CoordinatesEntity>(o => o.Coordinates, c =>
        {
            c.Property(p => p.Latitude).HasColumnName("latitude");
            c.Property(p => p.Longitude).HasColumnName("longitude");
        });

        builder.OwnsOne<AddressEntity>(o => o.Address, a =>
        {
            a.Property(p => p.ShortAddress).HasColumnName("address").HasMaxLength(500);
            a.Property(p => p.FullAddress).HasColumnName("full_address").HasMaxLength(1000);
        });

        builder.HasMany<PhoneEntity>(o => o.Phones)
               .WithOne()
               .HasForeignKey(p => p.OfficeId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indices for frequently queried columns
        builder.HasIndex(o => o.CityCode).HasDatabaseName("ix_offices_city_code");
        builder.HasIndex(o => o.Code).HasDatabaseName("ix_offices_code");
        builder.HasIndex(o => o.CountryCode).HasDatabaseName("ix_offices_country_code");
        builder.HasIndex(o => o.Type).HasDatabaseName("ix_offices_type");
    }
}
