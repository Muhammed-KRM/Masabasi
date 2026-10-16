namespace Masabasi.Data.Entities;

/// <summary>
/// Masabaşı Oyun Kulübü envanterindeki kutu oyunları, parti oyunları ve FRP sistemlerini temsil eden varlık.
/// </summary>
public sealed class BoardGame : BaseEntity
{
    /// <summary>Oyunun resmi adı (Örn: "Catan", "Secret Hitler / Kavgam", "Dungeons & Dragons 5e").</summary>
    public required string Title { get; set; }

    /// <summary>SEO uyumlu bağlantı kısa adı (Örn: "secret-hitler-kurallari-ve-taktikleri").</summary>
    public required string Slug { get; set; }

    /// <summary>Oyun kategorisi: "BoardGame", "SocialDeduction", "FRP_DnD", "CardGame".</summary>
    public required string Category { get; set; }

    /// <summary>Minimum oyuncu sayısı.</summary>
    public int MinPlayers { get; set; }

    /// <summary>Maksimum oyuncu sayısı.</summary>
    public int MaxPlayers { get; set; }

    /// <summary>En ideal oyuncu sayısı (Örn: "5-7 Oyuncu").</summary>
    public string? RecommendedPlayerCount { get; set; }

    /// <summary>Ortalama oyun süresi (dakika).</summary>
    public int PlayTimeMinutes { get; set; }

    /// <summary>BGG (BoardGameGeek) benzeri karmaşıklık/ağırlık derecesi (1.0 = Çok Kolay / Parti, 5.0 = Ağır Strateji).</summary>
    public double ComplexityRating { get; set; }

    /// <summary>Yapay zeka (GEO) ve hızlı bilgi paneli için 40-60 kelimelik özet (TL;DR).</summary>
    public required string TldrSummary { get; set; }

    /// <summary>Markdown formatında detaylı Türkçe kural rehberi ve oynanış taktikleri.</summary>
    public required string RulesMarkdown { get; set; }

    /// <summary>Kapak görseli bağlantı yolu.</summary>
    public string? ImageUrl { get; set; }

    /// <summary>Oyun kulübün fiziksel envanterinde mevcut mu?</summary>
    public bool IsInClubInventory { get; set; } = true;

    /// <summary>Oyunun yayınlanma yılı.</summary>
    public int? YearPublished { get; set; }
}
