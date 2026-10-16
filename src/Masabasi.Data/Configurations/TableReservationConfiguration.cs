using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masabasi.Data.Configurations;

/// <summary>
/// TableReservation varlığı için PostgreSQL Fluent API tablo ve kısıt yapılandırması.
/// </summary>
public sealed class TableReservationConfiguration : IEntityTypeConfiguration<TableReservation>
{
    public void Configure(EntityTypeBuilder<TableReservation> builder)
    {
        builder.ToTable("table_reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlayerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ContactHandle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ExperienceLevel)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.GameTableId, x.ContactHandle });
    }
}
