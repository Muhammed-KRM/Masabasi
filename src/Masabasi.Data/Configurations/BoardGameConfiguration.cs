using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masabasi.Data.Configurations;

/// <summary>
/// BoardGame varlığı için PostgreSQL Fluent API tablo ve indeks yapılandırması.
/// </summary>
public sealed class BoardGameConfiguration : IEntityTypeConfiguration<BoardGame>
{
    public void Configure(EntityTypeBuilder<BoardGame> builder)
    {
        builder.ToTable("board_games");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(180);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.RecommendedPlayerCount)
            .HasMaxLength(50);

        builder.Property(x => x.TldrSummary)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.RulesMarkdown)
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.IsInClubInventory);
    }
}
