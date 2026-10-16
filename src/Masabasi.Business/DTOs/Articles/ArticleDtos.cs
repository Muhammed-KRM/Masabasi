namespace Masabasi.Business.DTOs.Articles;

/// <summary>
/// Blog ve rehber listeleme ekranları için hafif özet DTO.
/// </summary>
public sealed record ArticleSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string TopicCluster,
    string TldrSummary,
    string MetaDescription,
    string AuthorName,
    DateTimeOffset PublishedAtUtc,
    int EstimatedReadTimeMinutes,
    string? CoverImageUrl);

/// <summary>
/// Blog veya rehber detay sayfası için zengin içerik ve HTML çevirisi içeren DTO.
/// </summary>
public sealed record ArticleDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string TopicCluster,
    string TldrSummary,
    string MetaDescription,
    string ContentMarkdown,
    string ContentHtml,
    string AuthorName,
    DateTimeOffset PublishedAtUtc,
    int EstimatedReadTimeMinutes,
    string? FaqJson,
    string? CoverImageUrl,
    int ViewCount);

/// <summary>
/// Admin panelinden SEO uyumlu yeni bir rehber veya blog yazısı ekleme modeli.
/// </summary>
public sealed record CreateArticleDto(
    string Title,
    string Slug,
    string TopicCluster,
    string TldrSummary,
    string MetaDescription,
    string ContentMarkdown,
    string AuthorName,
    string? FaqJson,
    string? CoverImageUrl);

/// <summary>
/// Ön yüz için sayfa SEO meta başlık ve şema verileri modeli.
/// </summary>
public sealed record SeoMetadataDto(
    string Title,
    string MetaDescription,
    string CanonicalUrl,
    string OgType,
    string? OgImageUrl,
    string? SchemaJsonLd);
