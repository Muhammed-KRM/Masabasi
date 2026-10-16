using FluentAssertions;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Masabasi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Masabasi.UnitTests.Data;

/// <summary>
/// GenericRepository tüm CRUD, filtreleme, sayma ve LINQ sorgulama metotlarını doğrulayan birim testleri.
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

    private static BoardGame CreateTestGame(string title, string slug, string category, int minP = 2, int maxP = 4) => new()
    {
        Title = title,
        Slug = slug,
        Category = category,
        MinPlayers = minP,
        MaxPlayers = maxP,
        PlayTimeMinutes = 60,
        ComplexityRating = 2.0,
        TldrSummary = $"{title} hakkında kısa özet bilgi.",
        RulesMarkdown = "## Kurallar\nKurallar metni burada."
    };

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var game = CreateTestGame("Catan", "catan", "BoardGame");
        await repo.AddAsync(game);
        await repo.SaveChangesAsync();

        var result = await repo.GetByIdAsync(game.Id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(game.Id);
        result.Title.Should().Be("Catan");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityIsSoftDeleted()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var game = CreateTestGame("DeletedGame", "deleted-game", "BoardGame");
        await repo.AddAsync(game);
        await repo.SaveChangesAsync();

        repo.Delete(game);
        await repo.SaveChangesAsync();

        var result = await repo.GetByIdAsync(game.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenTableIsEmpty()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var result = await repo.GetAllAsync();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveEntities()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var game1 = CreateTestGame("Game 1", "game-1", "Category A");
        var game2 = CreateTestGame("Game 2", "game-2", "Category B");
        var game3 = CreateTestGame("Game 3", "game-3", "Category A");

        await repo.AddRangeAsync([game1, game2, game3]);
        await repo.SaveChangesAsync();

        repo.Delete(game2);
        await repo.SaveChangesAsync();

        var list = await repo.GetAllAsync();
        list.Should().HaveCount(2);
        list.Should().Contain(g => g.Id == game1.Id);
        list.Should().Contain(g => g.Id == game3.Id);
        list.Should().NotContain(g => g.Id == game2.Id);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEntities()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        await repo.AddRangeAsync([
            CreateTestGame("Avalon", "avalon", "SocialDeduction"),
            CreateTestGame("Secret Hitler", "secret-hitler", "SocialDeduction"),
            CreateTestGame("Carcassonne", "carcassonne", "BoardGame")
        ]);
        await repo.SaveChangesAsync();

        var socialGames = await repo.FindAsync(g => g.Category == "SocialDeduction");
        socialGames.Should().HaveCount(2);
        socialGames.Should().OnlyContain(g => g.Category == "SocialDeduction");

        var emptyResults = await repo.FindAsync(g => g.Category == "NonExistentCategory");
        emptyResults.Should().BeEmpty();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnFirstMatchOrNull()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var catan = CreateTestGame("Catan", "catan", "BoardGame");
        await repo.AddAsync(catan);
        await repo.SaveChangesAsync();

        var found = await repo.FirstOrDefaultAsync(g => g.Slug == "catan");
        found.Should().NotBeNull();
        found!.Title.Should().Be("Catan");

        var notFound = await repo.FirstOrDefaultAsync(g => g.Slug == "olmayan-slug");
        notFound.Should().BeNull();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrueWhenMatchExists_AndFalseOtherwise()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var game = CreateTestGame("D&D 5e", "dnd-5e", "FRP_DnD");
        await repo.AddAsync(game);
        await repo.SaveChangesAsync();

        var exists = await repo.AnyAsync(g => g.Category == "FRP_DnD");
        exists.Should().BeTrue();

        var notExists = await repo.AnyAsync(g => g.Category == "NonExistent");
        notExists.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnAccurateCounts()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        (await repo.CountAsync()).Should().Be(0);

        await repo.AddRangeAsync([
            CreateTestGame("Game 1", "g1", "Strategy"),
            CreateTestGame("Game 2", "g2", "Party"),
            CreateTestGame("Game 3", "g3", "Strategy")
        ]);
        await repo.SaveChangesAsync();

        (await repo.CountAsync()).Should().Be(3);
        (await repo.CountAsync(g => g.Category == "Strategy")).Should().Be(2);
        (await repo.CountAsync(g => g.Category == "Party")).Should().Be(1);
        (await repo.CountAsync(g => g.Category == "RPG")).Should().Be(0);
    }

    [Fact]
    public async Task Update_ShouldModifyEntityValues()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        var game = CreateTestGame("Initial Title", "slug-init", "Category A");
        await repo.AddAsync(game);
        await repo.SaveChangesAsync();

        game.Title = "Updated Title";
        game.Category = "Category B";
        repo.Update(game);
        await repo.SaveChangesAsync();

        var updated = await repo.GetByIdAsync(game.Id);
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Updated Title");
        updated.Category.Should().Be("Category B");
    }

    [Fact]
    public async Task Query_ShouldSupportCustomLinqExpressions()
    {
        await using var context = CreateInMemoryDbContext();
        var repo = new GenericRepository<BoardGame>(context);

        await repo.AddRangeAsync([
            CreateTestGame("A Game", "a-game", "Cat", minP: 2, maxP: 4),
            CreateTestGame("B Game", "b-game", "Cat", minP: 4, maxP: 8),
            CreateTestGame("C Game", "c-game", "Cat", minP: 3, maxP: 6)
        ]);
        await repo.SaveChangesAsync();

        var sortedByMaxPlayers = await repo.Query()
            .OrderByDescending(g => g.MaxPlayers)
            .Take(2)
            .ToListAsync();

        sortedByMaxPlayers.Should().HaveCount(2);
        sortedByMaxPlayers[0].Title.Should().Be("B Game");
        sortedByMaxPlayers[1].Title.Should().Be("C Game");
    }
}
