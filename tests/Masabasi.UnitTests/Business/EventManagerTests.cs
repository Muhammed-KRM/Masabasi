using FluentAssertions;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Interfaces;
using Masabasi.Business.Services;
using Masabasi.Business.Validators;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// EventManager iş mantığını, masa rezervasyonu kurallarını, overbooking korumasını ve çakışma durumlarını doğrulayan testler.
/// </summary>
public sealed class EventManagerTests
{
    private static (MasabasiDbContext Db, EventManager Manager, Mock<IStaticSnapshotService> SnapshotMock) CreateTestFixture()
    {
        var dbOptions = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var db = new MasabasiDbContext(dbOptions);

        var services = new ServiceCollection();
        #pragma warning disable EXTEXP0018
        services.AddHybridCache();
        #pragma warning restore EXTEXP0018
        var sp = services.BuildServiceProvider();
        var hybridCache = sp.GetRequiredService<HybridCache>();

        var snapshotMock = new Mock<IStaticSnapshotService>();
        var reservationValidator = new CreateReservationValidator();
        var eventValidator = new CreateEventValidator();
        var logger = NullLogger<EventManager>.Instance;

        var manager = new EventManager(
            db,
            snapshotMock.Object,
            hybridCache,
            reservationValidator,
            eventValidator,
            logger);

        return (db, manager, snapshotMock);
    }

    [Fact]
    public async Task ReserveTableSeatAsync_ShouldSucceed_WhenSeatsAreAvailable()
    {
        // Arrange
        var (db, manager, snapshotMock) = CreateTestFixture();

        var clubEvent = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = "Cumartesi Buluşması",
            Slug = "cumartesi-bulusmasi",
            TldrSummary = "Haftalık kutu oyunu buluşması.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(2),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(2).AddHours(6),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy / İstanbul",
            IsActive = true
        };

        var table = new GameTable
        {
            Id = Guid.NewGuid(),
            ClubEventId = clubEvent.Id,
            TableNumber = 1,
            GameTitle = "Secret Hitler",
            Category = "SocialDeduction",
            HostOrDmName = "Moderatör",
            MaxPlayers = 10,
            CurrentPlayers = 8,
            IsBeginnerFriendly = true,
            ClubEvent = clubEvent
        };

        await db.ClubEvents.AddAsync(clubEvent);
        await db.GameTables.AddAsync(table);
        await db.SaveChangesAsync();

        var request = new CreateReservationDto(
            GameTableId: table.Id,
            PlayerName: "Ali Yılmaz",
            ContactHandle: "@ali_yilmaz",
            ExperienceLevel: "Beginner",
            Notes: "İlk defa oynayacağım.");

        // Act
        var result = await manager.ReserveTableSeatAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.CurrentPlayers.Should().Be(9);
        result.Data.MaxPlayers.Should().Be(10);

        // Snapshot servisinin çağrıldığı doğrulanır
        snapshotMock.Verify(s => s.ExportEventsSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReserveTableSeatAsync_ShouldFail_WhenTableIsAlreadyFull()
    {
        // Arrange
        var (db, manager, _) = CreateTestFixture();

        var clubEvent = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = "D&D Gecesi",
            Slug = "dnd-gecesi",
            TldrSummary = "D&D masası.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(1),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(1).AddHours(5),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy / İstanbul",
            IsActive = true
        };

        var table = new GameTable
        {
            Id = Guid.NewGuid(),
            ClubEventId = clubEvent.Id,
            TableNumber = 2,
            GameTitle = "D&D 5e: Curse of Strahd",
            Category = "FRP_DnD",
            HostOrDmName = "DM Kerem",
            MaxPlayers = 5,
            CurrentPlayers = 5, // Masa tamamen dolu!
            ClubEvent = clubEvent
        };

        await db.ClubEvents.AddAsync(clubEvent);
        await db.GameTables.AddAsync(table);
        await db.SaveChangesAsync();

        var request = new CreateReservationDto(
            GameTableId: table.Id,
            PlayerName: "Geciken Oyuncu",
            ContactHandle: "05329998877",
            ExperienceLevel: "Intermediate",
            Notes: null);

        // Act
        var result = await manager.ReserveTableSeatAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.ErrorMessage.Should().Contain("kontenjanı tamamen dolmuştur");
    }

    [Fact]
    public async Task ReserveTableSeatAsync_ShouldFail_WhenDuplicateContactHandleOnSameTable()
    {
        // Arrange
        var (db, manager, _) = CreateTestFixture();

        var clubEvent = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = "Catan Masası",
            Slug = "catan-masasi",
            TldrSummary = "Catan oyunu.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(1),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(1).AddHours(5),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy / İstanbul",
            IsActive = true
        };

        var table = new GameTable
        {
            Id = Guid.NewGuid(),
            ClubEventId = clubEvent.Id,
            TableNumber = 3,
            GameTitle = "Catan",
            Category = "BoardGame",
            HostOrDmName = "Anlatıcı",
            MaxPlayers = 4,
            CurrentPlayers = 1,
            ClubEvent = clubEvent
        };

        var existingReservation = new TableReservation
        {
            Id = Guid.NewGuid(),
            GameTableId = table.Id,
            PlayerName = "Mert Demir",
            ContactHandle = "@mertdemir",
            ExperienceLevel = "Veteran"
        };

        await db.ClubEvents.AddAsync(clubEvent);
        await db.GameTables.AddAsync(table);
        await db.TableReservations.AddAsync(existingReservation);
        await db.SaveChangesAsync();

        // Aynı iletişim bilgisi ile tekrar kayıt isteği
        var duplicateRequest = new CreateReservationDto(
            GameTableId: table.Id,
            PlayerName: "Mert Demir (Çift Kayıt)",
            ContactHandle: "@mertdemir",
            ExperienceLevel: "Veteran",
            Notes: null);

        // Act
        var result = await manager.ReserveTableSeatAsync(duplicateRequest);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.ErrorMessage.Should().Contain("zaten aktif bir kaydınız bulunuyor");
    }
}
