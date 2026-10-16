using FluentAssertions;
using Masabasi.API.Controllers.Public;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Games;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// GamesPublicController halka açık kütüphane endpoint testleri.
/// </summary>
public sealed class GamesPublicControllerTests
{
    private readonly Mock<IGameCatalogService> _catalogServiceMock = new();

    [Fact]
    public async Task GetGames_ShouldReturnOk_WithGameList()
    {
        var games = new List<BoardGameDto>
        {
            new(Guid.NewGuid(), "Catan", "catan", "BoardGame", 3, 4, "4", 75, 2.3, "Özet", "Kurallar", null, true, 1995)
        };

        _catalogServiceMock
            .Setup(s => s.GetGamesAsync(It.IsAny<GameFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<IReadOnlyList<BoardGameDto>>.Success(games));

        var controller = new GamesPublicController(_catalogServiceMock.Object);

        var response = await controller.GetGames(new GameFilterDto(), CancellationToken.None);

        var ok = response as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var data = ok.Value as IReadOnlyList<BoardGameDto>;
        data.Should().HaveCount(1);
        data![0].Title.Should().Be("Catan");
    }

    [Fact]
    public async Task GetGameBySlug_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        _catalogServiceMock
            .Setup(s => s.GetGameBySlugAsync("olmayan-oyun", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<BoardGameDto>.Failure("Oyun bulunamadı.", 404));

        var controller = new GamesPublicController(_catalogServiceMock.Object);

        var response = await controller.GetGameBySlug("olmayan-oyun", CancellationToken.None);

        var notFound = response as ObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(404);
    }
}
