using FluentAssertions;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Masabasi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Masabasi.UnitTests.Data;

/// <summary>
/// GenericRepository ve MasabasiDbContext audit/soft delete davranışlarını doğrulayan birim testleri.
/// </summary>
public sealed class GenericRepositoryTests
{
    private static MasabasiDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MasabasiDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAtUtcAutomatically_AndPersistEntity()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();
        var repository = new GenericRepository<BoardGame>(context);

        var game = new BoardGame
        {
            Title = "Catan",
            Slug = "catan-kutu-oyunu",
            Category = "BoardGame",
            MinPlayers = 3,
            MaxPlayers = 4,
            PlayTimeMinutes = 75,
            ComplexityRating = 2.3,
            TldrSummary = "Catan strateji kutu oyunu.",
            RulesMarkdown = "Catan adası kuralları..."
        };

        // Act
        await repository.AddAsync(game);
        await repository.SaveChangesAsync();

        // Assert
        var savedGame = await repository.GetByIdAsync(game.Id);
        savedGame.Should().NotBeNull();
        savedGame!.CreatedAtUtc.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        savedGame.Title.Should().Be("Catan");
    }

    [Fact]
    public async Task Delete_ShouldMarkEntityAsSoftDeleted_AndFilterFromQueries()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();
        var repository = new GenericRepository<BoardGame>(context);

        var game = new BoardGame
        {
            Title = "Secret Hitler",
            Slug = "secret-hitler",
            Category = "SocialDeduction",
            MinPlayers = 5,
            MaxPlayers = 10,
            PlayTimeMinutes = 45,
            ComplexityRating = 2.1,
            TldrSummary = "Sosyal çıkarım oyunu.",
            RulesMarkdown = "Kurallar..."
        };

        await repository.AddAsync(game);
        await repository.SaveChangesAsync();

        // Act
        repository.Delete(game);
        await repository.SaveChangesAsync();

        // Assert
        var activeGames = await repository.GetAllAsync();
        activeGames.Should().NotContain(g => g.Id == game.Id);

        // IgnoreQueryFilters ile veritabanında fiziksel olarak kaldığı doğrulanır
        var rawGame = await context.BoardGames.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == game.Id);
        rawGame.Should().NotBeNull();
        rawGame!.IsDeleted.Should().BeTrue();
    }
}
