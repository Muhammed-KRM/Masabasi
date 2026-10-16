using FluentAssertions;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Masabasi.UnitTests.Data;

/// <summary>
/// MasabasiDbContext otomatik audit (CreatedAt/UpdatedAt) ve tüm varlıklar için soft delete filtrelerini test eden sınıf.
/// </summary>
public sealed class DbContextAuditTests
{
    private static MasabasiDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MasabasiDbContext(options);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetUpdatedAtUtc_OnEntityModification()
    {
        await using var context = CreateInMemoryDbContext();

        var article = new Article
        {
            Title = "İlk Başlık Uzunluğu Yeterli",
            Slug = "ilk-baslik-yeterli",
            TopicCluster = "KutuOyunlari",
            TldrSummary = "Özet bilgi en az 20 kelime olmalı ve kurallara uymalıdır. Masabaşı Oyun Kulübü için test amaçlı yazılmıştır.",
            MetaDescription = "Meta açıklaması 90 karakterden uzun olmalıdır ve test amacıyla hazırlanmış bir açıklamadır.",
            ContentMarkdown = "## İçerik\nEn az yüz karakter olmalıdır.",
            AuthorName = "Yazar",
            IsPublished = true
        };

        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();

        var initialCreated = article.CreatedAtUtc;
        article.UpdatedAtUtc.Should().BeNull();

        // Modifiye et
        article.Title = "Güncellenen Başlık";
        await context.SaveChangesAsync();

        article.UpdatedAtUtc.Should().NotBeNull();
        article.UpdatedAtUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        article.CreatedAtUtc.Should().Be(initialCreated);
    }

    [Fact]
    public void SaveChanges_Synchronous_ShouldApplyAuditTimestamps()
    {
        using var context = CreateInMemoryDbContext();

        var game = new BoardGame
        {
            Title = "Senkron Test Oyunu",
            Slug = "senkron-test-oyunu",
            Category = "BoardGame",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 30,
            ComplexityRating = 1.5,
            TldrSummary = "Senkron audit testi için kısa özet.",
            RulesMarkdown = "Kurallar..."
        };

        context.BoardGames.Add(game);
        context.SaveChanges();

        game.CreatedAtUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SoftDeleteFilter_ShouldExcludeDeletedEntities_AcrossAllDbSets()
    {
        await using var context = CreateInMemoryDbContext();

        var clubEvent = new ClubEvent
        {
            Title = "Silinecek Etkinlik",
            Slug = "silinecek-etkinlik",
            TldrSummary = "Özet",
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy",
            IsDeleted = true
        };

        var table = new GameTable
        {
            TableNumber = 99,
            GameTitle = "Silinecek Masa",
            Category = "BoardGame",
            HostOrDmName = "DM",
            MaxPlayers = 5,
            IsDeleted = true
        };

        var reservation = new TableReservation
        {
            PlayerName = "Silinecek Oyuncu",
            ContactHandle = "@silindi",
            IsDeleted = true
        };

        var game = new BoardGame
        {
            Title = "Silinecek Oyun",
            Slug = "silinecek-oyun",
            Category = "BoardGame",
            TldrSummary = "Özet",
            RulesMarkdown = "Kurallar",
            IsDeleted = true
        };

        var article = new Article
        {
            Title = "Silinecek Makale",
            Slug = "silinecek-makale",
            TopicCluster = "KutuOyunlari",
            TldrSummary = "Özet",
            MetaDescription = "Açıklama",
            ContentMarkdown = "İçerik",
            AuthorName = "Yazar",
            IsDeleted = true
        };

        await context.ClubEvents.AddAsync(clubEvent);
        await context.GameTables.AddAsync(table);
        await context.TableReservations.AddAsync(reservation);
        await context.BoardGames.AddAsync(game);
        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();

        // Varsayılan sorgularda hiçbiri gelmemeli
        (await context.ClubEvents.CountAsync()).Should().Be(0);
        (await context.GameTables.CountAsync()).Should().Be(0);
        (await context.TableReservations.CountAsync()).Should().Be(0);
        (await context.BoardGames.CountAsync()).Should().Be(0);
        (await context.Articles.CountAsync()).Should().Be(0);

        // IgnoreQueryFilters ile hepsinin veritabanında fiziksel olarak kaldığı doğrulanır
        (await context.ClubEvents.IgnoreQueryFilters().CountAsync()).Should().Be(1);
        (await context.GameTables.IgnoreQueryFilters().CountAsync()).Should().Be(1);
        (await context.TableReservations.IgnoreQueryFilters().CountAsync()).Should().Be(1);
        (await context.BoardGames.IgnoreQueryFilters().CountAsync()).Should().Be(1);
        (await context.Articles.IgnoreQueryFilters().CountAsync()).Should().Be(1);
    }
}
