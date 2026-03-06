using DellinTerminalImporter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DellinTerminalImporter.Data.Configurations;

public class PhoneConfiguration : IEntityTypeConfiguration<PhoneEntity>
{
    public void Configure(EntityTypeBuilder<PhoneEntity> builder)
    {
        builder.ToTable("phones");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Number).HasMaxLength(100).IsRequired();
        builder.Property(p => p.PhoneType).HasColumnName("type").HasMaxLength(50);
        builder.Property(p => p.Comment).HasMaxLength(200);

        builder.HasIndex(p => p.OfficeId).HasDatabaseName("ix_phones_office_id");
    }
}
