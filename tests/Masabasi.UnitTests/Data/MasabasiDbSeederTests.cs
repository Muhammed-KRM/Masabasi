using FluentAssertions;
using Masabasi.Data.Context;
using Masabasi.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Masabasi.UnitTests.Data;

/// <summary>
/// MasabasiDbSeeder tohumlama mantığını doğrulayan birim testleri.
/// </summary>
public sealed class MasabasiDbSeederTests
{
    private static MasabasiDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MasabasiDbContext(options);
    }

    [Fact]
    public async Task SeedAsync_ShouldPopulateBoardGamesEventsTablesAndArticles()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();

        // Act
        await MasabasiDbSeeder.SeedAsync(context);

        // Assert
        var gamesCount = await context.BoardGames.CountAsync();
        var eventsCount = await context.ClubEvents.CountAsync();
        var tablesCount = await context.GameTables.CountAsync();
        var articlesCount = await context.Articles.CountAsync();

        gamesCount.Should().BeGreaterThanOrEqualTo(5);
        eventsCount.Should().BeGreaterThanOrEqualTo(1);
        tablesCount.Should().BeGreaterThanOrEqualTo(4);
        articlesCount.Should().BeGreaterThanOrEqualTo(3);

        var firstEvent = await context.ClubEvents.Include(e => e.Tables).FirstOrDefaultAsync();
        firstEvent.Should().NotBeNull();
        firstEvent!.Tables.Should().NotBeEmpty();
    }
}
