using FluentAssertions;
using Masabasi.API.Controllers.Public;
using Masabasi.API.Hubs;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// ReservationsPublicController uç noktasının SignalR bildirimi ve ServiceResult eşlemesini test eden sınıf.
/// </summary>
public sealed class ReservationsPublicControllerTests
{
    private readonly Mock<IEventService> _eventServiceMock = new();
    private readonly Mock<IHubContext<TableHub>> _hubContextMock = new();
    private readonly Mock<IHubClients> _hubClientsMock = new();
    private readonly Mock<IClientProxy> _clientProxyMock = new();

    public ReservationsPublicControllerTests()
    {
        _hubClientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
        _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnSuccess_AndBroadcastSignalRUpdate()
    {
        // Arrange
        var request = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: "Burak Alp",
            ContactHandle: "@burakalp",
            ExperienceLevel: "Beginner",
            Notes: null);

        var reservationResult = new ReservationResultDto(
            ReservationId: Guid.NewGuid(),
            TableId: request.GameTableId,
            TableNumber: 2,
            GameTitle: "Secret Hitler",
            CurrentPlayers: 5,
            MaxPlayers: 10);

        _eventServiceMock
            .Setup(s => s.ReserveTableSeatAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<ReservationResultDto>.Success(reservationResult));

        var controller = new ReservationsPublicController(
            _eventServiceMock.Object,
            _hubContextMock.Object,
            NullLogger<ReservationsPublicController>.Instance);

        // Act
        var actionResult = await controller.CreateReservation(request, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var returnedData = okResult.Value as ReservationResultDto;
        returnedData.Should().NotBeNull();
        returnedData!.TableNumber.Should().Be(2);

        // SignalR All.SendCoreAsync çağrısının tetiklendiği doğrulanır
        _clientProxyMock.Verify(
            c => c.SendCoreAsync(
                "TableSeatUpdated",
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
