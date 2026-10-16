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
/// EventManager iş mantığını, masa rezervasyonu kurallarını, overbooking korumasını, etkinlik/masa yönetimini ve önbellekleme durumlarını doğrulayan testler.
/// </summary>
public sealed class EventManagerTests
{
    private static (MasabasiDbContext Db, EventManager Manager, Mock<IStaticSnapshotService> SnapshotMock, HybridCache Cache) CreateTestFixture()
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

        return (db, manager, snapshotMock, hybridCache);
    }

    [Fact]
    public async Task ReserveTableSeatAsync_ShouldSucceed_WhenSeatsAreAvailable()
    {
        var (db, manager, snapshotMock, _) = CreateTestFixture();

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

        var result = await manager.ReserveTableSeatAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.CurrentPlayers.Should().Be(9);
        result.Data.MaxPlayers.Should().Be(10);

        snapshotMock.Verify(s => s.ExportEventsSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReserveTableSeatAsync_ShouldFail_WhenTableIsAlreadyFull()
    {
        var (db, manager, _, _) = CreateTestFixture();

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
            CurrentPlayers = 5,
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

        var result = await manager.ReserveTableSeatAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.ErrorMessage.Should().Contain("kontenjanı tamamen dolmuştur");
    }

    [Fact]
    public async Task ReserveTableSeatAsync_ShouldFail_WhenDuplicateContactHandleOnSameTable()
    {
        var (db, manager, _, _) = CreateTestFixture();

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

        var duplicateRequest = new CreateReservationDto(
            GameTableId: table.Id,
            PlayerName: "Mert Demir (Çift Kayıt)",
            ContactHandle: "@mertdemir",
            ExperienceLevel: "Veteran",
            Notes: null);

        var result = await manager.ReserveTableSeatAsync(duplicateRequest);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.ErrorMessage.Should().Contain("zaten aktif bir kaydınız bulunuyor");
    }

    [Fact]
    public async Task GetActiveEventsCachedAsync_ShouldReturnActiveEventsList()
    {
        var (db, manager, _, _) = CreateTestFixture();

        var ev = new ClubEvent
        {
            Title = "Aktif Cumartesi Etkinliği",
            Slug = "aktif-etkinlik",
            TldrSummary = "Özet bilgi.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(3),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(3).AddHours(6),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy",
            IsActive = true,
            Tables = new List<GameTable>
            {
                new()
                {
                    TableNumber = 1,
                    GameTitle = "Catan",
                    Category = "BoardGame",
                    HostOrDmName = "Host",
                    MaxPlayers = 4,
                    CurrentPlayers = 1
                }
            }
        };

        await db.ClubEvents.AddAsync(ev);
        await db.SaveChangesAsync();

        var result1 = await manager.GetActiveEventsCachedAsync();
        result1.IsSuccess.Should().BeTrue();
        result1.Data.Should().HaveCount(1);
        result1.Data![0].Title.Should().Be("Aktif Cumartesi Etkinliği");

        // İkinci çağrı HybridCache üzerinden gelmeli
        var result2 = await manager.GetActiveEventsCachedAsync();
        result2.IsSuccess.Should().BeTrue();
        result2.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetEventBySlugAsync_ShouldReturnEventOrNotFound()
    {
        var (db, manager, _, _) = CreateTestFixture();

        var ev = new ClubEvent
        {
            Title = "Özel Slug Etkinliği",
            Slug = "ozel-slug",
            TldrSummary = "Özet",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(1),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(1).AddHours(5),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy",
            IsActive = true
        };
        await db.ClubEvents.AddAsync(ev);
        await db.SaveChangesAsync();

        var found = await manager.GetEventBySlugAsync("ozel-slug");
        found.IsSuccess.Should().BeTrue();
        found.Data!.Title.Should().Be("Özel Slug Etkinliği");

        var notFound = await manager.GetEventBySlugAsync("olmayan-slug");
        notFound.IsSuccess.Should().BeFalse();
        notFound.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetEventByIdAsync_ShouldReturnEventOrNotFound()
    {
        var (db, manager, _, _) = CreateTestFixture();

        var id = Guid.NewGuid();
        var ev = new ClubEvent
        {
            Id = id,
            Title = "ID Etkinliği",
            Slug = "id-etkinligi",
            TldrSummary = "Özet",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(1),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(1).AddHours(5),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy",
            IsActive = true
        };
        await db.ClubEvents.AddAsync(ev);
        await db.SaveChangesAsync();

        var found = await manager.GetEventByIdAsync(id);
        found.IsSuccess.Should().BeTrue();

        var notFound = await manager.GetEventByIdAsync(Guid.NewGuid());
        notFound.IsSuccess.Should().BeFalse();
        notFound.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldSucceed_AndFailOnDuplicateSlug()
    {
        var (db, manager, snapshotMock, _) = CreateTestFixture();

        var request = new CreateEventDto(
            Title: "Yeni Cumartesi Etkinliği",
            Slug: "yeni-cumartesi-etkinligi",
            TldrSummary: "Yeni haftalık kutu oyunu ve FRP buluşması.",
            StartDateUtc: DateTimeOffset.UtcNow.AddDays(4),
            EndDateUtc: DateTimeOffset.UtcNow.AddDays(4).AddHours(6),
            VenueName: "Kadıköy",
            VenueAddress: "Kadıköy / İstanbul",
            MeetupEventUrl: null,
            ExternalRegistrationUrl: null);

        var result = await manager.CreateEventAsync(request);
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        snapshotMock.Verify(s => s.ExportAllSnapshotsAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Mükerrer slug ile tekrar deneme
        var duplicateResult = await manager.CreateEventAsync(request);
        duplicateResult.IsSuccess.Should().BeFalse();
        duplicateResult.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task AddTableAsync_ShouldAddTableToEvent_AndFailOnDuplicateNumber()
    {
        var (db, manager, snapshotMock, _) = CreateTestFixture();

        var ev = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = "Masa Eklenecek Etkinlik",
            Slug = "masa-eklenecek-etkinlik",
            TldrSummary = "Özet",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(2),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(2).AddHours(6),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy",
            IsActive = true
        };
        await db.ClubEvents.AddAsync(ev);
        await db.SaveChangesAsync();

        var tableRequest = new CreateGameTableDto(
            ClubEventId: ev.Id,
            TableNumber: 1,
            GameTitle: "Secret Hitler",
            Category: "SocialDeduction",
            HostOrDmName: "Moderatör",
            MaxPlayers: 10,
            IsBeginnerFriendly: true);

        var result = await manager.AddTableAsync(tableRequest);
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        snapshotMock.Verify(s => s.ExportEventsSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Aynı etkinlikte aynı masa numarası
        var duplicateResult = await manager.AddTableAsync(tableRequest);
        duplicateResult.IsSuccess.Should().BeFalse();
        duplicateResult.StatusCode.Should().Be(409);

        // Olmayan etkinlik
        var notFoundResult = await manager.AddTableAsync(tableRequest with { ClubEventId = Guid.NewGuid() });
        notFoundResult.IsSuccess.Should().BeFalse();
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CancelReservationAsync_ShouldDecrementCurrentPlayers_AndSoftDelete()
    {
        var (db, manager, snapshotMock, _) = CreateTestFixture();

        var table = new GameTable
        {
            Id = Guid.NewGuid(),
            TableNumber = 1,
            GameTitle = "Avalon",
            Category = "SocialDeduction",
            HostOrDmName = "DM",
            MaxPlayers = 10,
            CurrentPlayers = 3
        };

        var reservation = new TableReservation
        {
            Id = Guid.NewGuid(),
            GameTableId = table.Id,
            PlayerName = "Vazgeçen Oyuncu",
            ContactHandle = "@vazgecen",
            GameTable = table
        };

        await db.GameTables.AddAsync(table);
        await db.TableReservations.AddAsync(reservation);
        await db.SaveChangesAsync();

        var cancelResult = await manager.CancelReservationAsync(reservation.Id);
        cancelResult.IsSuccess.Should().BeTrue();

        table.CurrentPlayers.Should().Be(2);
        reservation.IsDeleted.Should().BeTrue();
        snapshotMock.Verify(s => s.ExportEventsSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Olmayan rezervasyon iptali
        var notFoundCancel = await manager.CancelReservationAsync(Guid.NewGuid());
        notFoundCancel.IsSuccess.Should().BeFalse();
        notFoundCancel.StatusCode.Should().Be(404);
    }
}
