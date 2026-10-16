using FluentAssertions;
using Masabasi.Business.DTOs.Games;
using Masabasi.Business.Interfaces;
using Masabasi.Business.Services;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// GameCatalogManager kutu oyunu kataloğu arama, filtreleme, ekleme ve güncelleme testleri.
/// </summary>
public sealed class GameCatalogManagerTests
{
    private static (MasabasiDbContext Db, GameCatalogManager Manager, Mock<IStaticSnapshotService> SnapshotMock) CreateTestFixture()
    {
        var dbOptions = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var db = new MasabasiDbContext(dbOptions);
        var snapshotMock = new Mock<IStaticSnapshotService>();
        var manager = new GameCatalogManager(db, snapshotMock.Object, NullLogger<GameCatalogManager>.Instance);

        return (db, manager, snapshotMock);
    }

    private static BoardGame CreateGame(string title, string slug, string category, int minP, int maxP, double complexity, bool inInventory = true) => new()
    {
        Title = title,
        Slug = slug,
        Category = category,
        MinPlayers = minP,
        MaxPlayers = maxP,
        PlayTimeMinutes = 60,
        ComplexityRating = complexity,
        TldrSummary = $"{title} özet açıklaması.",
        RulesMarkdown = "Kurallar...",
        IsInClubInventory = inInventory
    };

    [Fact]
    public async Task GetGamesAsync_ShouldFilterAccuratelyByMultipleCriteria()
    {
        var (db, manager, _) = CreateTestFixture();

        await db.BoardGames.AddRangeAsync([
            CreateGame("Catan", "catan", "BoardGame", minP: 3, maxP: 4, complexity: 2.3, inInventory: true),
            CreateGame("Secret Hitler", "secret-hitler", "SocialDeduction", minP: 5, maxP: 10, complexity: 2.1, inInventory: true),
            CreateGame("Brass: Birmingham", "brass-birmingham", "BoardGame", minP: 2, maxP: 4, complexity: 3.9, inInventory: false),
            CreateGame("D&D 5e", "dnd-5e", "FRP_DnD", minP: 3, maxP: 6, complexity: 3.0, inInventory: true)
        ]);
        await db.SaveChangesAsync();

        // 1. Kategori filtresi
        var social = await manager.GetGamesAsync(new GameFilterDto(Category: "SocialDeduction"));
        social.Data.Should().HaveCount(1);
        social.Data![0].Title.Should().Be("Secret Hitler");

        // 2. Oyuncu sayısı (5 kişi oynayabilecek oyunlar: Secret Hitler & D&D)
        var forFive = await manager.GetGamesAsync(new GameFilterDto(PlayerCount: 5));
        forFive.Data.Should().HaveCount(2);

        // 3. Maksimum zorluk (<= 2.5)
        var easy = await manager.GetGamesAsync(new GameFilterDto(MaxComplexity: 2.5));
        easy.Data.Should().HaveCount(2);

        // 4. Sadece kulüp envanteri
        var inventoryOnly = await manager.GetGamesAsync(new GameFilterDto(OnlyInInventory: true));
        inventoryOnly.Data.Should().HaveCount(3);
        inventoryOnly.Data.Should().NotContain(g => g.Title == "Brass: Birmingham");

        // 5. Arama terimi
        var searchResult = await manager.GetGamesAsync(new GameFilterDto(SearchTerm: "hitler"));
        searchResult.Data.Should().HaveCount(1);
        searchResult.Data![0].Title.Should().Be("Secret Hitler");
    }

    [Fact]
    public async Task GetGameBySlugAsync_ShouldReturnGameOrNotFound()
    {
        var (db, manager, _) = CreateTestFixture();

        await db.BoardGames.AddAsync(CreateGame("Avalon", "avalon", "SocialDeduction", 5, 10, 2.4));
        await db.SaveChangesAsync();

        var found = await manager.GetGameBySlugAsync("avalon");
        found.IsSuccess.Should().BeTrue();
        found.Data!.Title.Should().Be("Avalon");

        var notFound = await manager.GetGameBySlugAsync("olmayan-oyun");
        notFound.IsSuccess.Should().BeFalse();
        notFound.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldPersist_AndFailOnDuplicateSlug()
    {
        var (db, manager, snapshotMock) = CreateTestFixture();

        var request = new CreateBoardGameDto(
            Title: "Terraforming Mars",
            Slug: "terraforming-mars",
            Category: "BoardGame",
            MinPlayers: 1,
            MaxPlayers: 5,
            RecommendedPlayerCount: "3-4 Oyuncu",
            PlayTimeMinutes: 120,
            ComplexityRating: 3.2,
            TldrSummary: "Mars'ı yaşanabilir kılma strateji oyunu.",
            RulesMarkdown: "## Kurallar",
            ImageUrl: null,
            IsInClubInventory: true,
            YearPublished: 2016);

        var result = await manager.CreateGameAsync(request);
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);

        snapshotMock.Verify(s => s.ExportGamesSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);
        snapshotMock.Verify(s => s.ExportLlmsTxtAsync(It.IsAny<CancellationToken>()), Times.Once);
        snapshotMock.Verify(s => s.ExportSitemapXmlAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Mükerrer slug
        var duplicate = await manager.CreateGameAsync(request);
        duplicate.IsSuccess.Should().BeFalse();
        duplicate.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task UpdateGameAsync_ShouldUpdateFields_AndReturnNotFoundOnMissing()
    {
        var (db, manager, snapshotMock) = CreateTestFixture();

        var game = CreateGame("Eski İsim", "eski-isim", "BoardGame", 2, 4, 2.0);
        await db.BoardGames.AddAsync(game);
        await db.SaveChangesAsync();

        var updateRequest = new CreateBoardGameDto(
            Title: "Yeni Güncel İsim",
            Slug: "eski-isim",
            Category: "PartyGame",
            MinPlayers: 3,
            MaxPlayers: 8,
            RecommendedPlayerCount: "5 Oyuncu",
            PlayTimeMinutes: 45,
            ComplexityRating: 1.8,
            TldrSummary: "Yeni özet.",
            RulesMarkdown: "Yeni kurallar.",
            ImageUrl: null,
            IsInClubInventory: false,
            YearPublished: 2020);

        var updateResult = await manager.UpdateGameAsync(game.Id, updateRequest);
        updateResult.IsSuccess.Should().BeTrue();

        var updated = await manager.GetGameBySlugAsync("eski-isim");
        updated.Data!.Title.Should().Be("Yeni Güncel İsim");
        updated.Data.Category.Should().Be("PartyGame");
        updated.Data.IsInClubInventory.Should().BeFalse();

        // Olmayan oyun güncellemesi
        var notFound = await manager.UpdateGameAsync(Guid.NewGuid(), updateRequest);
        notFound.IsSuccess.Should().BeFalse();
        notFound.StatusCode.Should().Be(404);
    }
}
