using FluentAssertions;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Masabasi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Masabasi.UnitTests.Data;

/// <summary>
/// EventRepository özel metodlarını (Include ile masa ve rezervasyon sorguları) doğrulayan birim testleri.
/// </summary>
public sealed class EventRepositoryTests
{
    private static MasabasiDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MasabasiDbContext(options);
    }

    [Fact]
    public async Task GetUpcomingEventsWithTablesAsync_ShouldReturnActiveEventsWithTheirTables()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();
        var repository = new EventRepository(context);

        var activeEvent = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = "Masabaşı Cumartesi Buluşması #40",
            Slug = "cumartesi-bulusmasi-40",
            TldrSummary = "Haftalık kutu oyunu ve FRP gecesi.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(2),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(2).AddHours(6),
            VenueName = "Kadıköy Buluşma Mekanı",
            VenueAddress = "Kadıköy / İstanbul",
            IsActive = true,
            Tables = new List<GameTable>
            {
                new()
                {
                    TableNumber = 1,
                    GameTitle = "Secret Hitler",
                    Category = "SocialDeduction",
                    HostOrDmName = "DM Can",
                    MaxPlayers = 10,
                    CurrentPlayers = 6,
                    IsBeginnerFriendly = true
                },
                new()
                {
                    TableNumber = 2,
                    GameTitle = "D&D 5e: Curse of Strahd",
                    Category = "FRP_DnD",
                    HostOrDmName = "DM Kerem",
                    MaxPlayers = 5,
                    CurrentPlayers = 4,
                    IsBeginnerFriendly = false
                }
            }
        };

        var passiveEvent = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = "Eski İptal Edilen Etkinlik",
            Slug = "eski-etkinlik",
            TldrSummary = "Eski etkinlik.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(-10),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(-10).AddHours(6),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy / İstanbul",
            IsActive = false
        };

        await context.ClubEvents.AddRangeAsync(activeEvent, passiveEvent);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetUpcomingEventsWithTablesAsync(count: 5);

        // Assert
        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Masabaşı Cumartesi Buluşması #40");
        result[0].Tables.Should().HaveCount(2);
        result[0].Tables.First().GameTitle.Should().Be("Secret Hitler");
    }

    [Fact]
    public async Task GetEventWithTablesBySlugAsync_ShouldReturnEventBySlugWithTablesAndReservations()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();
        var repository = new EventRepository(context);

        var eventId = Guid.NewGuid();
        var clubEvent = new ClubEvent
        {
            Id = eventId,
            Title = "Masabaşı Özel Turnuva",
            Slug = "masabasi-ozel-turnuva",
            TldrSummary = "Özel turnuva etkinliği.",
            StartDateUtc = DateTimeOffset.UtcNow.AddDays(4),
            EndDateUtc = DateTimeOffset.UtcNow.AddDays(4).AddHours(5),
            VenueName = "Kadıköy",
            VenueAddress = "Kadıköy",
            IsActive = true,
            Tables = new List<GameTable>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TableNumber = 1,
                    GameTitle = "Catan Turnuva Masası",
                    Category = "BoardGame",
                    HostOrDmName = "Moderatör",
                    MaxPlayers = 4,
                    CurrentPlayers = 1,
                    Reservations = new List<TableReservation>
                    {
                        new()
                        {
                            PlayerName = "Mehmet S.",
                            ContactHandle = "05551234567",
                            ExperienceLevel = "Veteran"
                        }
                    }
                }
            }
        };

        await context.ClubEvents.AddAsync(clubEvent);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetEventWithTablesBySlugAsync("masabasi-ozel-turnuva");

        // Assert
        result.Should().NotBeNull();
        result!.Slug.Should().Be("masabasi-ozel-turnuva");
        result.Tables.Should().HaveCount(1);
        result.Tables.First().Reservations.Should().HaveCount(1);
        result.Tables.First().Reservations.First().PlayerName.Should().Be("Mehmet S.");
    }
}
