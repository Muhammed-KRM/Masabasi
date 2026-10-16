using System.Text.Json;
using FluentAssertions;
using Masabasi.API.Hubs;
using Masabasi.API.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// TableHub SignalR grup yönetimi ve GlobalExceptionMiddleware küresel hata yakalama davranışlarını test eden sınıf.
/// </summary>
public sealed class TableHubAndMiddlewareTests
{
    [Fact]
    public async Task TableHub_JoinAndLeaveEventRoom_ShouldInteractWithGroupManager()
    {
        var groupManagerMock = new Mock<IGroupManager>();
        var hubCallerContextMock = new Mock<HubCallerContext>();
        hubCallerContextMock.Setup(c => c.ConnectionId).Returns("conn-123");

        var hub = new TableHub(NullLogger<TableHub>.Instance)
        {
            Groups = groupManagerMock.Object,
            Context = hubCallerContextMock.Object
        };

        // Act
        await hub.JoinEventRoom("cumartesi-bulusmasi");
        await hub.LeaveEventRoom("cumartesi-bulusmasi");

        // Assert
        groupManagerMock.Verify(g => g.AddToGroupAsync("conn-123", "event:cumartesi-bulusmasi", It.IsAny<CancellationToken>()), Times.Once);
        groupManagerMock.Verify(g => g.RemoveFromGroupAsync("conn-123", "event:cumartesi-bulusmasi", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GlobalExceptionMiddleware_ShouldPassThrough_WhenNoExceptionOccurs()
    {
        var context = new DefaultHttpContext();
        bool nextCalled = false;

        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            ctx.Response.StatusCode = 200;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionMiddleware(next, NullLogger<GlobalExceptionMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GlobalExceptionMiddleware_ShouldCatchException_AndReturnProblemDetails500()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (ctx) => throw new InvalidOperationException("Simüle edilen veritabanı çökmesi!");

        var middleware = new GlobalExceptionMiddleware(next, NullLogger<GlobalExceptionMiddleware>.Instance);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Contain("application/problem+json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        string responseBody = await reader.ReadToEndAsync();

        var problem = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
        problem.Title.Should().Be("Sunucu Hatası");
        problem.Detail.Should().Contain("beklenmeyen bir hata oluştu");

        // Hassas hata detayının sızdırılmadığı doğrulanır
        responseBody.Should().NotContain("Simüle edilen veritabanı çökmesi!");
    }
}
