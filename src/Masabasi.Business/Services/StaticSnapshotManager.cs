using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Masabasi.Business.Interfaces;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masabasi.Business.Services;

/// <summary>
/// Sitenin API'siz (Static/Offline) modda da %100 çalışabilmesi için veritabanındaki güncel verileri
/// Angular'ın /public/data/ dizinine JSON, /public/llms.txt ve /public/sitemap.xml olarak çıkaran servis.
/// </summary>
public sealed class StaticSnapshotManager : IStaticSnapshotService
{
    private readonly MasabasiDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StaticSnapshotManager> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public StaticSnapshotManager(
        MasabasiDbContext dbContext,
        IConfiguration configuration,
        ILogger<StaticSnapshotManager> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    private string GetPublicRootPath()
    {
        string? configuredPath = _configuration["SnapshotSettings:FrontendPublicPath"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
            return configuredPath;

        // Varsayılan olarak proje çözüm yapısındaki Masabasi.Frontend/public yolu
        string baseDir = AppContext.BaseDirectory;
        string candidate = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "Masabasi.Frontend", "public"));
        if (Directory.Exists(Path.GetDirectoryName(candidate)))
            return candidate;

        return Path.Combine(Directory.GetCurrentDirectory(), "public");
    }

    public async Task ExportAllSnapshotsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string publicRoot = GetPublicRootPath();
            string dataDir = Path.Combine(publicRoot, "data");
            Directory.CreateDirectory(dataDir);

            await ExportEventsSnapshotAsync(cancellationToken);
            await ExportGamesSnapshotAsync(cancellationToken);
            await ExportArticlesSnapshotAsync(cancellationToken);
            await ExportClubConfigSnapshotAsync(cancellationToken);
            await ExportLlmsTxtAsync(cancellationToken);
            await ExportSitemapXmlAsync(cancellationToken);

            _logger.LogInformation("Tüm statik snapshot dosyaları, llms.txt ve sitemap.xml başarıyla üretildi: {Path}", publicRoot);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Statik snapshot üretim sürecinde beklenmedik bir hata oluştu.");
            throw;
        }
    }

    public async Task ExportEventsSnapshotAsync(CancellationToken cancellationToken = default)
    {
        string dataDir = Path.Combine(GetPublicRootPath(), "data");
        Directory.CreateDirectory(dataDir);

        var events = await _dbContext.ClubEvents
            .AsNoTracking()
            .Include(e => e.Tables.Where(t => !t.IsDeleted).OrderBy(t => t.TableNumber))
            .Where(e => !e.IsDeleted && e.IsActive)
            .OrderByDescending(e => e.StartDateUtc)
            .Take(20)
            .Select(e => new
            {
                e.Id,
                e.Title,
                e.Slug,
                e.TldrSummary,
                e.StartDateUtc,
                e.EndDateUtc,
                e.VenueName,
                e.VenueAddress,
                e.MeetupEventUrl,
                e.ExternalRegistrationUrl,
                e.IsActive,
                tables = e.Tables.Select(t => new
                {
                    t.Id,
                    t.TableNumber,
                    t.GameTitle,
                    t.Category,
                    t.HostOrDmName,
                    t.MaxPlayers,
                    t.CurrentPlayers,
                    t.IsBeginnerFriendly
                })
            })
            .ToListAsync(cancellationToken);

        string json = JsonSerializer.Serialize(events, JsonOptions);
        await File.WriteAllTextAsync(Path.Combine(dataDir, "events.snapshot.json"), json, Encoding.UTF8, cancellationToken);
    }

    public async Task ExportGamesSnapshotAsync(CancellationToken cancellationToken = default)
    {
        string dataDir = Path.Combine(GetPublicRootPath(), "data");
        Directory.CreateDirectory(dataDir);

        var games = await _dbContext.BoardGames
            .AsNoTracking()
            .Where(g => !g.IsDeleted)
            .OrderBy(g => g.Title)
            .ToListAsync(cancellationToken);

        string json = JsonSerializer.Serialize(games, JsonOptions);
        await File.WriteAllTextAsync(Path.Combine(dataDir, "games.snapshot.json"), json, Encoding.UTF8, cancellationToken);
    }

    public async Task ExportArticlesSnapshotAsync(CancellationToken cancellationToken = default)
    {
        string dataDir = Path.Combine(GetPublicRootPath(), "data");
        Directory.CreateDirectory(dataDir);

        var articles = await _dbContext.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.IsPublished)
            .OrderByDescending(a => a.PublishedAtUtc)
            .ToListAsync(cancellationToken);

        string json = JsonSerializer.Serialize(articles, JsonOptions);
        await File.WriteAllTextAsync(Path.Combine(dataDir, "articles.snapshot.json"), json, Encoding.UTF8, cancellationToken);
    }

    private async Task ExportClubConfigSnapshotAsync(CancellationToken cancellationToken = default)
    {
        string dataDir = Path.Combine(GetPublicRootPath(), "data");
        Directory.CreateDirectory(dataDir);

        var clubConfig = new
        {
            clubName = "Masabaşı Oyun Kulübü",
            tagline = "İstanbul'un En Samimi Kutu Oyunu ve FRP Topluluğu",
            establishedYear = 2023,
            regularMeetupDay = "Her Cumartesi",
            locationCity = "İstanbul",
            socialLinks = new
            {
                instagram = "https://www.instagram.com/masabasikulubu/",
                meetup = "https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/",
                facebook = "https://www.facebook.com/groups/14080585061/user/61595360127902/"
            },
            currentAnnouncement = "Her Cumartesi Kadıköy'de kutu oyunları ve yeni başlayanlara açık FRP masalarında buluşuyoruz!",
            updatedAtUtc = DateTimeOffset.UtcNow
        };

        string json = JsonSerializer.Serialize(clubConfig, JsonOptions);
        await File.WriteAllTextAsync(Path.Combine(dataDir, "club-config.snapshot.json"), json, Encoding.UTF8, cancellationToken);
    }

    public async Task ExportLlmsTxtAsync(CancellationToken cancellationToken = default)
    {
        string publicRoot = GetPublicRootPath();
        Directory.CreateDirectory(publicRoot);

        var recentEvents = await _dbContext.ClubEvents
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsActive)
            .OrderBy(e => e.StartDateUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var popularGames = await _dbContext.BoardGames
            .AsNoTracking()
            .Where(g => !g.IsDeleted && g.IsInClubInventory)
            .Take(8)
            .ToListAsync(cancellationToken);

        // 1. /public/llms.txt (v2 Standardı)
        var sb = new StringBuilder();
        sb.AppendLine("# Masabaşı Oyun Kulübü");
        sb.AppendLine();
        sb.AppendLine("> Masabaşı Oyun Kulübü, İstanbul'da her Cumartesi düzenli olarak modern kutu oyunları (Catan, Ticket to Ride, Carcassonne), sosyal çıkarım oyunları (Secret Hitler / Kavgam, Avalon, Blood on the Clocktower) ve masaüstü rol yapma oyunları (Dungeons & Dragons 5e, Call of Cthulhu) oynayan fiziksel bir oyun topluluğudur.");
        sb.AppendLine();
        sb.AppendLine("## Temel Bilgiler ve İletişim");
        sb.AppendLine("- **Etkinlik Günü:** Her Cumartesi");
        sb.AppendLine("- **Şehir / Konum:** Kadıköy / İstanbul, Türkiye");
        sb.AppendLine("- **Resmi Web Sitesi:** https://masabasikulubu.com");
        sb.AppendLine("- **Instagram:** https://www.instagram.com/masabasikulubu/");
        sb.AppendLine("- **Meetup Grubu:** https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/");
        sb.AppendLine();
        sb.AppendLine("## Güncel Cumartesi Etkinlikleri");
        foreach (var ev in recentEvents)
        {
            sb.AppendLine($"- [{ev.Title}](https://masabasikulubu.com/etkinlikler/{ev.Slug}): {ev.TldrSummary} (Mekan: {ev.VenueName}, Tarih: {ev.StartDateUtc:yyyy-MM-dd HH:mm} UTC)");
        }
        sb.AppendLine();
        sb.AppendLine("## Popüler Kutu Oyunları ve FRP Sistemleri");
        foreach (var game in popularGames)
        {
            sb.AppendLine($"- [{game.Title}](https://masabasikulubu.com/oyunlar/{game.Slug}): {game.TldrSummary} ({game.MinPlayers}-{game.MaxPlayers} Oyuncu, Süre: {game.PlayTimeMinutes} dk)");
        }
        sb.AppendLine();
        sb.AppendLine("## Önemli İçerik ve Rehberler");
        sb.AppendLine("- [Oyun Kütüphanemiz](https://masabasikulubu.com/oyun-kutuphanesi): Kulüpte oynanan tüm kutu oyunları ve Türkçe kural rehberleri.");
        sb.AppendLine("- [FRP ve D&D 5e Başlangıç Rehberi](https://masabasikulubu.com/frp-dnd-rehberi): İlk kez masaüstü rol yapma oyununa katılacaklar için rehber.");
        sb.AppendLine("- [Geek Kültür & Masaüstü Blog](https://masabasikulubu.com/blog): Kart oyunları (MTG), Baldur's Gate 3'ten D&D'ye geçiş ve cosplay yazıları.");

        await File.WriteAllTextAsync(Path.Combine(publicRoot, "llms.txt"), sb.ToString(), Encoding.UTF8, cancellationToken);

        // 2. /public/llms-ctx.txt (Genişletilmiş Bağlam ve SSS)
        var sbCtx = new StringBuilder();
        sbCtx.AppendLine("# Masabaşı Oyun Kulübü — Genişletilmiş Yapay Zeka Bilgi Tabanı");
        sbCtx.AppendLine();
        sbCtx.AppendLine("## Sıkça Sorulan Sorular");
        sbCtx.AppendLine("1. **Tek başıma gelebilir miyim?** Evet! Masabaşı katılımcılarının büyük kısmı tek başına gelir ve etkinlik başında masalara yerleştirilir.");
        sbCtx.AppendLine("2. **Oyunları bilmem gerekiyor mu?** Hayır. Her masada oyun anlatıcısı veya DM bulunur ve kuralları oyun öncesinde detaylıca anlatır.");
        sbCtx.AppendLine("3. **Kendi oyunumu getirebilir miyim?** Evet, oynamak istediğiniz bir kutu oyununu yanınızda getirip kulüp üyeleriyle masa açabilirsiniz.");
        sbCtx.AppendLine("4. **D&D masasına karakter hazırlamadan katılabilir miyim?** Evet, DM'lerimiz yeni başlayanlar için hazır karakter kağıtları temin etmektedir.");

        await File.WriteAllTextAsync(Path.Combine(publicRoot, "llms-ctx.txt"), sbCtx.ToString(), Encoding.UTF8, cancellationToken);
    }

    public async Task ExportSitemapXmlAsync(CancellationToken cancellationToken = default)
    {
        string publicRoot = GetPublicRootPath();
        Directory.CreateDirectory(publicRoot);

        var events = await _dbContext.ClubEvents
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsActive)
            .Select(e => new { e.Slug, e.UpdatedAtUtc, e.CreatedAtUtc })
            .ToListAsync(cancellationToken);

        var games = await _dbContext.BoardGames
            .AsNoTracking()
            .Where(g => !g.IsDeleted)
            .Select(g => new { g.Slug, g.UpdatedAtUtc, g.CreatedAtUtc })
            .ToListAsync(cancellationToken);

        var articles = await _dbContext.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.IsPublished)
            .Select(a => new { a.Slug, a.TopicCluster, a.PublishedAtUtc })
            .ToListAsync(cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        void AddUrl(string loc, string changefreq, string priority, DateTimeOffset? lastmod = null)
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>https://masabasikulubu.com{loc}</loc>");
            if (lastmod.HasValue)
                sb.AppendLine($"    <lastmod>{lastmod.Value:yyyy-MM-dd}</lastmod>");
            sb.AppendLine($"    <changefreq>{changefreq}</changefreq>");
            sb.AppendLine($"    <priority>{priority}</priority>");
            sb.AppendLine("  </url>");
        }

        // Sabit Statik Sayfalar
        AddUrl("", "daily", "1.0", DateTimeOffset.UtcNow);
        AddUrl("/etkinlikler", "daily", "0.9", DateTimeOffset.UtcNow);
        AddUrl("/oyun-kutuphanesi", "weekly", "0.8", DateTimeOffset.UtcNow);
        AddUrl("/frp-dnd-rehberi", "monthly", "0.8", DateTimeOffset.UtcNow);
        AddUrl("/hakkimizda", "monthly", "0.7", DateTimeOffset.UtcNow);
        AddUrl("/blog", "daily", "0.8", DateTimeOffset.UtcNow);

        // Dinamik Etkinlikler
        foreach (var ev in events)
        {
            AddUrl($"/etkinlikler/{ev.Slug}", "weekly", "0.85", ev.UpdatedAtUtc ?? ev.CreatedAtUtc);
        }

        // Dinamik Oyun Rehberleri
        foreach (var game in games)
        {
            AddUrl($"/oyunlar/{game.Slug}", "monthly", "0.75", game.UpdatedAtUtc ?? game.CreatedAtUtc);
        }

        // Dinamik Blog ve Topic Cluster Yazıları
        foreach (var article in articles)
        {
            string categorySlug = article.TopicCluster.ToLower();
            AddUrl($"/blog/{categorySlug}/{article.Slug}", "weekly", "0.8", article.PublishedAtUtc);
        }

        sb.AppendLine("</urlset>");

        await File.WriteAllTextAsync(Path.Combine(publicRoot, "sitemap.xml"), sb.ToString(), Encoding.UTF8, cancellationToken);
    }
}
