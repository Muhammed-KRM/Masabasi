using System.Text.Json;
using FluentAssertions;
using Masabasi.Business.Services;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// StaticSnapshotManager servisinin API'siz çalışma için ürettiği statik JSON dosyaları,
/// /public/llms.txt ve /public/sitemap.xml çıktılarını doğrulayan birim testleri.
/// </summary>
public sealed class StaticSnapshotManagerTests : IDisposable
{
    private readonly string _tempDir;
    private readonly MasabasiDbContext _dbContext;

    public StaticSnapshotManagerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "masabasi_tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);

        var dbOptions = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new MasabasiDbContext(dbOptions);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }
        catch
        {
            // Test sonrası geçici dizin temizliği hatası göz ardı edilir
        }
    }

    [Fact]
    public async Task ExportAllSnapshotsAsync_ShouldGenerateAllSnapshotFilesAndLlmsTxt()
    {
        // Arrange
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["SnapshotSettings:FrontendPublicPath"] = _tempDir
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();

        // Test verileri ekle
        var testEvent = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = "Snapshot Test Etkinliği",
            Slug = "snapshot-test-etkinligi",
            TldrSummary = "Bu etkinlik snapshot doğrulaması içindir.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(3),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(3).AddHours(6),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy / İstanbul",
            IsActive = true,
            Tables = new List<GameTable>
            {
                new()
                {
                    TableNumber = 1,
                    GameTitle = "Catan",
                    Category = "BoardGame",
                    HostOrDmName = "Anlatıcı",
                    MaxPlayers = 4,
                    CurrentPlayers = 2,
                    IsBeginnerFriendly = true
                }
            }
        };

        var testGame = new BoardGame
        {
            Title = "Secret Hitler",
            Slug = "secret-hitler",
            Category = "SocialDeduction",
            MinPlayers = 5,
            MaxPlayers = 10,
            PlayTimeMinutes = 45,
            ComplexityRating = 2.1,
            TldrSummary = "Sosyal çıkarım oyunu.",
            RulesMarkdown = "Kurallar...",
            IsInClubInventory = true
        };

        var testArticle = new Article
        {
            Title = "D&D 5e Başlangıç Kılavuzu: Zarlar ve Karakterler",
            Slug = "dnd-5e-baslangic-kilavuzu",
            TopicCluster = "FrpDnD",
            TldrSummary = "D&D 5e oynamaya sıfırdan başlayanlar için temel zar ve kural kılavuzu.",
            MetaDescription = "D&D 5e başlangıç rehberi. Karakter nasıl açılır, d20 zarı ne işe yarar?",
            ContentMarkdown = "## D&D Giriş\nİlk adım karakter seçimidir.",
            AuthorName = "DM Kerem",
            IsPublished = true,
            PublishedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.ClubEvents.AddAsync(testEvent);
        await _dbContext.BoardGames.AddAsync(testGame);
        await _dbContext.Articles.AddAsync(testArticle);
        await _dbContext.SaveChangesAsync();

        var manager = new StaticSnapshotManager(_dbContext, configuration, NullLogger<StaticSnapshotManager>.Instance);

        // Act
        await manager.ExportAllSnapshotsAsync();

        // Assert - Dosyaların varlığı
        string dataDir = Path.Combine(_tempDir, "data");
        File.Exists(Path.Combine(dataDir, "events.snapshot.json")).Should().BeTrue();
        File.Exists(Path.Combine(dataDir, "games.snapshot.json")).Should().BeTrue();
        File.Exists(Path.Combine(dataDir, "articles.snapshot.json")).Should().BeTrue();
        File.Exists(Path.Combine(dataDir, "club-config.snapshot.json")).Should().BeTrue();
        File.Exists(Path.Combine(_tempDir, "llms.txt")).Should().BeTrue();
        File.Exists(Path.Combine(_tempDir, "llms-ctx.txt")).Should().BeTrue();
        File.Exists(Path.Combine(_tempDir, "sitemap.xml")).Should().BeTrue();

        // Assert - İçerik kontrolleri
        string eventsJson = await File.ReadAllTextAsync(Path.Combine(dataDir, "events.snapshot.json"));
        eventsJson.Should().Contain("Snapshot Test Etkinliği");
        eventsJson.Should().Contain("Catan");

        string llmsTxt = await File.ReadAllTextAsync(Path.Combine(_tempDir, "llms.txt"));
        llmsTxt.Should().Contain("# Masabaşı Oyun Kulübü");
        llmsTxt.Should().Contain("snapshot-test-etkinligi");

        string sitemapXml = await File.ReadAllTextAsync(Path.Combine(_tempDir, "sitemap.xml"));
        sitemapXml.Should().Contain("<loc>https://masabasikulubu.com/etkinlikler/snapshot-test-etkinligi</loc>");
        sitemapXml.Should().Contain("<loc>https://masabasikulubu.com/oyunlar/secret-hitler</loc>");
    }
}
