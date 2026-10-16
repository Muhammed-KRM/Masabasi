using FluentAssertions;
using Masabasi.API.Controllers.Public;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// EventsPublicController uç noktalarının HTTP yanıtlarını ve ServiceResult eşlemelerini doğrulayan testler.
/// </summary>
public sealed class EventsPublicControllerTests
{
    private readonly Mock<IEventService> _eventServiceMock = new();

    [Fact]
    public async Task GetActiveEvents_ShouldReturnOk_WithActiveEventsList()
    {
        // Arrange
        var eventsList = new List<EventDetailDto>
        {
            new(
                Guid.NewGuid(),
                "Masabaşı Buluşması #45",
                "masabasi-bulusmasi-45",
                "Özet bilgi.",
                DateTimeOffset.UtcNow.AddDays(2),
                DateTimeOffset.UtcNow.AddDays(2).AddHours(6),
                "Kadıköy Mekanı",
                "Kadıköy / İstanbul",
                null,
                null,
                new List<GameTableDto>())
        };

        _eventServiceMock
            .Setup(s => s.GetActiveEventsCachedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<IReadOnlyList<EventDetailDto>>.Success(eventsList));

        var controller = new EventsPublicController(_eventServiceMock.Object);

        // Act
        var actionResult = await controller.GetActiveEvents(CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var data = okResult.Value as IReadOnlyList<EventDetailDto>;
        data.Should().NotBeNull();
        data!.Should().HaveCount(1);
        data[0].Title.Should().Be("Masabaşı Buluşması #45");
    }

    [Fact]
    public async Task GetEventBySlug_ShouldReturnNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        _eventServiceMock
            .Setup(s => s.GetEventBySlugAsync("olmayan-etkinlik", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<EventDetailDto>.Failure("Etkinlik bulunamadı.", 404));

        var controller = new EventsPublicController(_eventServiceMock.Object);

        // Act
        var actionResult = await controller.GetEventBySlug("olmayan-etkinlik", CancellationToken.None);

        // Assert
        var objectResult = actionResult as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(404);
    }
}
