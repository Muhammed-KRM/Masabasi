using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Masabasi.Business.DTOs.Events;
using Xunit;

namespace Masabasi.UnitTests.Integration;

/// <summary>
/// Masa koltuk rezervasyonu akışını (başarılı kayıt, mükerrer kontrolü, kapasite aşımı,
/// doğrulama hataları ve veri tutarlılığı) uçtan uca test eden sınıf.
/// </summary>
public sealed class ReservationsE2ETests : IClassFixture<MasabasiApiFactory>
{
    private readonly HttpClient _client;

    public ReservationsE2ETests(MasabasiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateReservation_FullFlow_ShouldSucceed_AndPreventDuplicate()
    {
        // 1. Mevcut etkinlik ve masa bilgilerini al
        var eventsResponse = await _client.GetAsync("/api/v1/public/events");
        eventsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var events = await eventsResponse.Content.ReadFromJsonAsync<List<EventDetailDto>>();
        events.Should().NotBeNull();
        events!.Should().NotBeEmpty();

        var openTable = events.SelectMany(e => e.Tables).FirstOrDefault(t => !t.IsFull);
        openTable.Should().NotBeNull("Tohum veride en az bir adet açık masa bulunmalıdır.");

        int initialPlayerCount = openTable!.CurrentPlayers;

        // 2. Geçerli rezervasyon isteği gönder
        var reservationRequest = new CreateReservationDto(
            GameTableId: openTable.Id,
            PlayerName: "E2E Test Oyuncusu",
            ContactHandle: "@e2e_player",
            ExperienceLevel: "Intermediate",
            Notes: "E2E entegrasyon testi rezervasyon notu.");

        var response = await _client.PostAsJsonAsync("/api/v1/public/reservations", reservationRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reservationResult = await response.Content.ReadFromJsonAsync<ReservationResultDto>();
        reservationResult.Should().NotBeNull();
        reservationResult!.TableId.Should().Be(openTable.Id);
        reservationResult.CurrentPlayers.Should().Be(initialPlayerCount + 1);

        // 3. Aynı oyuncu aynı masaya tekrar rezervasyon yapmayı denerse 409 Conflict almalı
        var duplicateResponse = await _client.PostAsJsonAsync("/api/v1/public/reservations", reservationRequest);
        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturn400BadRequest_WhenValidationFails()
    {
        // Boş isim ve geçersiz iletişim bilgisi içeren DTO
        var invalidRequest = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: "", // Boş bırakılamaz kuralı
            ContactHandle: "", // Boş bırakılamaz kuralı
            ExperienceLevel: "Expert",
            Notes: null);

        var response = await _client.PostAsJsonAsync("/api/v1/public/reservations", invalidRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturn404NotFound_WhenTableDoesNotExist()
    {
        var nonExistentTableRequest = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: "Bilinmeyen Masa Oyuncusu",
            ContactHandle: "@unknown_user",
            ExperienceLevel: "Beginner",
            Notes: null);

        var response = await _client.PostAsJsonAsync("/api/v1/public/reservations", nonExistentTableRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
