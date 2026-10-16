namespace Masabasi.Data.Entities;

/// <summary>
/// Masabaşı Oyun Kulübü'nün SEO ve Topic Cluster (Hub & Spoke) mimarisine uygun blog, rehber ve köprü içeriklerini temsil eden varlık.
/// </summary>
public sealed class Article : BaseEntity
{
    /// <summary>Yazı başlığı (SEO standardı: 40-65 karakter, Örn: "D&D 5e Nasıl Başlanır? Karakter Kağıdı Hazırlama Rehberi").</summary>
    public required string Title { get; set; }

    /// <summary>SEO uyumlu URL kısa adı (Örn: "dnd-5e-nasil-baslanir-karakter-kagidi-rehberi").</summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Topic Cluster ana kategorisi:
    /// "KutuOyunlari", "FrpDnD", "KartOyunlari", "DijitaldenMasaustune", "CosplayAksesuar", "KulupHaberleri"
    /// </summary>
    public required string TopicCluster { get; set; }

    /// <summary>Yapay zeka arama motorları (GEO) ve okuyucular için 40-60 kelimelik net özet (TL;DR).</summary>
    public required string TldrSummary { get; set; }

    /// <summary>SEO meta açıklama etiketi (110-160 karakter).</summary>
    public required string MetaDescription { get; set; }

    /// <summary>Markdown formatında zengin içerik metni.</summary>
    public required string ContentMarkdown { get; set; }

    /// <summary>Yazar adı ve kulüpteki unvanı (Örn: "Efe (Dungeon Master) & Masabaşı Ekibi").</summary>
    public required string AuthorName { get; set; }

    /// <summary>Google FAQPage Schema.org oluşturmak için SSS verisi (JSON formatında Soru/Cevap listesi).</summary>
    public string? FaqJson { get; set; }

    /// <summary>Kapak görseli bağlantısı.</summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>Tahmini okuma süresi (dakika).</summary>
    public int EstimatedReadTimeMinutes { get; set; } = 5;

    /// <summary>Yazının yayında olma durumu.</summary>
    public bool IsPublished { get; set; } = true;

    /// <summary>Yayınlanma tarihi (UTC).</summary>
    public DateTimeOffset PublishedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Toplam görüntülenme sayısı.</summary>
    public int ViewCount { get; set; } = 0;
}
