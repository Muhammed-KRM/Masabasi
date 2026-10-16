namespace Masabasi.Business.Interfaces;

/// <summary>
/// Veritabanındaki güncel içerikleri Angular vitrininin API'siz (offline/statik) çalışabilmesi için
/// statik JSON dosyalarına (/public/data/*.snapshot.json), sitemap.xml ve /public/llms.txt dosyalarına aktaran servis sözleşmesi.
/// </summary>
public interface IStaticSnapshotService
{
    /// <summary>
    /// Tüm etkinlik, kütüphane, makale ve yapay zeka indeksleme (llms.txt) dosyalarını tek seferde diske aktarır.
    /// </summary>
    Task ExportAllSnapshotsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sadece etkinlikler ve masa kontenjanlarını events.snapshot.json olarak dışa aktarır.
    /// </summary>
    Task ExportEventsSnapshotAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Kutu oyunları ve FRP rehberlerini games.snapshot.json olarak dışa aktarır.
    /// </summary>
    Task ExportGamesSnapshotAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// SEO rehberlerini ve blog yazılarını articles.snapshot.json olarak dışa aktarır.
    /// </summary>
    Task ExportArticlesSnapshotAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Yapay zeka botları (GPTBot, ClaudeBot, Perplexity) için /llms.txt ve /llms-ctx.txt belgelerini üretir.
    /// </summary>
    Task ExportLlmsTxtAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Arama motorları için öncelikli /sitemap.xml haritasını üretir.
    /// </summary>
    Task ExportSitemapXmlAsync(CancellationToken cancellationToken = default);
}
