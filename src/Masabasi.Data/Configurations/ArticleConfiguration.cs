using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masabasi.Data.Configurations;

/// <summary>
/// Article varlığı için PostgreSQL Fluent API tablo ve indeks yapılandırması.
/// </summary>
public sealed class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(220);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.TopicCluster)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(x => x.TldrSummary)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.MetaDescription)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ContentMarkdown)
            .IsRequired();

        builder.Property(x => x.AuthorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CoverImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(x => x.TopicCluster);
        builder.HasIndex(x => x.IsPublished);
        builder.HasIndex(x => x.PublishedAtUtc);
    }
}
