using FluentAssertions;
using Masabasi.API.Controllers.Admin;
using Masabasi.API.Hubs;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.DTOs.Games;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// EventsAdminController, GamesAdminController ve ArticlesAdminController yönetim uçlarının birim testleri.
/// </summary>
public sealed class AdminControllersTests
{
    [Fact]
    public async Task EventsAdminController_CreateEvent_And_AddTable_And_CancelReservation()
    {
        var eventServiceMock = new Mock<IEventService>();
        var hubMock = new Mock<IHubContext<TableHub>>();
        var controller = new EventsAdminController(eventServiceMock.Object, hubMock.Object);

        // 1. CreateEvent
        var eventRequest = new CreateEventDto("Etkinlik", "etkinlik", "Özet", DateTimeOffset.UtcNow.AddDays(2), DateTimeOffset.UtcNow.AddDays(2).AddHours(5), "Kadıköy", "Kadıköy", null, null);
        eventServiceMock.Setup(s => s.CreateEventAsync(eventRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<Guid>.Success(Guid.NewGuid(), 201));

        var createResult = await controller.CreateEvent(eventRequest, CancellationToken.None) as ObjectResult;
        createResult.Should().NotBeNull();
        createResult!.StatusCode.Should().Be(201);

        // 2. AddTable
        var tableRequest = new CreateGameTableDto(Guid.NewGuid(), 1, "Catan", "BoardGame", "DM", 4, true);
        eventServiceMock.Setup(s => s.AddTableAsync(tableRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<Guid>.Success(Guid.NewGuid(), 201));

        var addTableResult = await controller.AddTable(tableRequest, CancellationToken.None) as ObjectResult;
        addTableResult.Should().NotBeNull();
        addTableResult!.StatusCode.Should().Be(201);

        // 3. CancelReservation
        var resId = Guid.NewGuid();
        eventServiceMock.Setup(s => s.CancelReservationAsync(resId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true));

        var cancelResult = await controller.CancelReservation(resId, CancellationToken.None) as OkObjectResult;
        cancelResult.Should().NotBeNull();
        cancelResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GamesAdminController_CreateGame_UpdateGame_AndDeleteGame()
    {
        var catalogMock = new Mock<IGameCatalogService>();
        var cacheStoreMock = new Mock<Microsoft.AspNetCore.OutputCaching.IOutputCacheStore>();
        var controller = new GamesAdminController(catalogMock.Object, cacheStoreMock.Object);

        var dto = new CreateBoardGameDto("Catan", "catan", "BoardGame", 3, 4, "4", 75, 2.3, "Özet", "Kurallar", null, true, 1995);

        // 1. CreateGame
        catalogMock.Setup(s => s.CreateGameAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<Guid>.Success(Guid.NewGuid(), 201));

        var createResult = await controller.CreateGame(dto, CancellationToken.None) as ObjectResult;
        createResult.Should().NotBeNull();
        createResult!.StatusCode.Should().Be(201);

        // 2. UpdateGame
        var gameId = Guid.NewGuid();
        catalogMock.Setup(s => s.UpdateGameAsync(gameId, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true));

        var updateResult = await controller.UpdateGame(gameId, dto, CancellationToken.None) as OkObjectResult;
        updateResult.Should().NotBeNull();
        updateResult!.StatusCode.Should().Be(200);

        // 3. DeleteGame
        catalogMock.Setup(s => s.DeleteGameAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true, 204));

        var deleteResult = await controller.DeleteGame(gameId, CancellationToken.None) as NoContentResult;
        deleteResult.Should().NotBeNull();
        deleteResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task ArticlesAdminController_CreateArticle_UpdateArticle_AndDeleteArticle()
    {
        var articleMock = new Mock<IArticleService>();
        var controller = new ArticlesAdminController(articleMock.Object);

        var dto = new CreateArticleDto("Başlık", "slug", "FrpDnD", "Özet", "Meta", "İçerik", "Yazar", null, null);

        // 1. CreateArticle
        articleMock.Setup(s => s.CreateArticleAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<Guid>.Success(Guid.NewGuid(), 201));

        var createResult = await controller.CreateArticle(dto, CancellationToken.None) as ObjectResult;
        createResult.Should().NotBeNull();
        createResult!.StatusCode.Should().Be(201);

        // 2. UpdateArticle
        var articleId = Guid.NewGuid();
        articleMock.Setup(s => s.UpdateArticleAsync(articleId, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true));

        var updateResult = await controller.UpdateArticle(articleId, dto, CancellationToken.None) as OkObjectResult;
        updateResult.Should().NotBeNull();
        updateResult!.StatusCode.Should().Be(200);

        // 3. DeleteArticle
        articleMock.Setup(s => s.DeleteArticleAsync(articleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true, 204));

        var deleteResult = await controller.DeleteArticle(articleId, CancellationToken.None) as NoContentResult;
        deleteResult.Should().NotBeNull();
        deleteResult!.StatusCode.Should().Be(204);
    }
}
