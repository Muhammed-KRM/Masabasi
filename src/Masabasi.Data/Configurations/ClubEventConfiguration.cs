using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masabasi.Data.Configurations;

/// <summary>
/// ClubEvent varlığı için PostgreSQL Fluent API tablo ve ilişki yapılandırması.
/// </summary>
public sealed class ClubEventConfiguration : IEntityTypeConfiguration<ClubEvent>
{
    public void Configure(EntityTypeBuilder<ClubEvent> builder)
    {
        builder.ToTable("club_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(220);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.TldrSummary)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.VenueName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.VenueAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.MeetupEventUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ExternalRegistrationUrl)
            .HasMaxLength(500);

        builder.HasMany(x => x.Tables)
            .WithOne(t => t.ClubEvent)
            .HasForeignKey(t => t.ClubEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StartDateUtc);
        builder.HasIndex(x => x.IsActive);
    }
}
