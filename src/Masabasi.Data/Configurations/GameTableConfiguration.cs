using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masabasi.Data.Configurations;

/// <summary>
/// GameTable varlığı için PostgreSQL Fluent API tablo ve eşzamanlılık (concurrency) yapılandırması.
/// </summary>
public sealed class GameTableConfiguration : IEntityTypeConfiguration<GameTable>
{
    public void Configure(EntityTypeBuilder<GameTable> builder)
    {
        builder.ToTable("game_tables");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GameTitle)
            .IsRequired()
            .HasMaxLength(160);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.HostOrDmName)
            .IsRequired()
            .HasMaxLength(100);

        // PostgreSQL xmin sistem sütunu ile Optimistic Concurrency (Son koltuğa aynı anda 2 kişi tıklarsa 2. kişiyi engeller)
        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => new { x.ClubEventId, x.TableNumber })
            .IsUnique();

        builder.HasMany(x => x.Reservations)
            .WithOne(r => r.GameTable)
            .HasForeignKey(r => r.GameTableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
