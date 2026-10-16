using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.DTOs.Games;
using Xunit;

namespace Masabasi.UnitTests.Integration;

/// <summary>
/// Masabaşı Yönetim (Admin) API uç noktalarını (Etkinlik ve Masa Yönetimi, Oyun Kataloğu CRUD,
/// Makale İçerik CRUD ve Statik Snapshot Dışa Aktarımı) uçtan uca test eden sınıf.
/// </summary>
public sealed class AdminEndpointsE2ETests : IClassFixture<MasabasiApiFactory>
{
    private readonly HttpClient _client;

    public AdminEndpointsE2ETests(MasabasiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Admin Events and Tables Management

    [Fact]
    public async Task AdminEvents_CreateEventAndAddTable_FullFlow()
    {
        // 1. Yeni etkinlik oluştur
        string uniqueSlug = "yeni-yonetim-etkinligi-" + Guid.NewGuid().ToString("N")[..8];
        var eventDto = new CreateEventDto(
            Title: "Haftalık Masaüstü Rol Yapma Buluşması",
            Slug: uniqueSlug,
            TldrSummary: "Kulüp üyeleri için düzenlenen haftalık rol yapma ve kutu oyunu gecesi.",
            StartDateUtc: DateTimeOffset.UtcNow.AddDays(7),
            EndDateUtc: DateTimeOffset.UtcNow.AddDays(7).AddHours(5),
            VenueName: "Masabaşı Kadıköy Kulüp Evi",
            VenueAddress: "Caferağa Mah. Moda Cad. No:42 Kadıköy / İstanbul",
            MeetupEventUrl: null,
            ExternalRegistrationUrl: null);

        var createEventResponse = await _client.PostAsJsonAsync("/api/v1/admin/events", eventDto);
        createEventResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var eventId = await createEventResponse.Content.ReadFromJsonAsync<Guid>();
        eventId.Should().NotBeEmpty();

        // 2. Oluşturulan etkinliğe yeni masa ekle
        var tableDto = new CreateGameTableDto(
            ClubEventId: eventId,
            TableNumber: 10,
            GameTitle: "Dungeons & Dragons 5E",
            Category: "FRP",
            HostOrDmName: "DM Burak",
            MaxPlayers: 6,
            IsBeginnerFriendly: true);

        var addTableResponse = await _client.PostAsJsonAsync("/api/v1/admin/events/tables", tableDto);
        addTableResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var tableId = await addTableResponse.Content.ReadFromJsonAsync<Guid>();
        tableId.Should().NotBeEmpty();

        // 3. Public uçtan sorgulayarak oluşturulan etkinliğin ve masanın listelendiğini doğrula
        var publicCheckResponse = await _client.GetAsync($"/api/v1/public/events/{uniqueSlug}");
        publicCheckResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventDetail = await publicCheckResponse.Content.ReadFromJsonAsync<EventDetailDto>();
        eventDetail.Should().NotBeNull();
        eventDetail!.Tables.Should().Contain(t => t.TableNumber == 10 && t.GameTitle == "Dungeons & Dragons 5E");
    }

    #endregion

    #region Admin Games CRUD

    [Fact]
    public async Task AdminGames_CreateUpdateDelete_FullFlow()
    {
        string uniqueSlug = "admin-test-oyun-" + Guid.NewGuid().ToString("N")[..8];

        // 1. Create Game
        var createDto = new CreateBoardGameDto(
            Title: "Kanban EV Strateji",
            Slug: uniqueSlug,
            Category: "HeavyStrategy",
            MinPlayers: 1,
            MaxPlayers: 4,
            RecommendedPlayerCount: "3-4 Oyuncu",
            PlayTimeMinutes: 150,
            ComplexityRating: 4.4,
            TldrSummary: "Elektrikli araç üretim hattı verimlilik simülasyonu.",
            RulesMarkdown: "## Kurallar\nSandra'nın denetimlerine dikkat edin.",
            ImageUrl: null,
            IsInClubInventory: true,
            YearPublished: 2020);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/admin/games", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var gameId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        gameId.Should().NotBeEmpty();

        // 2. Update Game
        var updateDto = createDto with
        {
            Title = "Kanban EV Güncellenmiş Baskı",
            PlayTimeMinutes = 180
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/admin/games/{gameId}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Public uçtan güncellemenin yansıdığını kontrol et
        var getResponse = await _client.GetAsync($"/api/v1/public/games/{uniqueSlug}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var gameDetail = await getResponse.Content.ReadFromJsonAsync<BoardGameDto>();
        gameDetail.Should().NotBeNull();
        gameDetail!.Title.Should().Be("Kanban EV Güncellenmiş Baskı");
        gameDetail.PlayTimeMinutes.Should().Be(180);

        // 4. Delete Game (Soft Delete)
        var deleteResponse = await _client.DeleteAsync($"/api/v1/admin/games/{gameId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 5. Silinen oyun artık public uçtan erişilemez olmalıdır
        var deletedCheck = await _client.GetAsync($"/api/v1/public/games/{uniqueSlug}");
        deletedCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Admin Articles CRUD

    [Fact]
    public async Task AdminArticles_CreateUpdateDelete_FullFlow()
    {
        string uniqueSlug = "admin-test-makale-" + Guid.NewGuid().ToString("N")[..8];

        // 1. Create Article
        var createDto = new CreateArticleDto(
            Title: "D&D 5E Büyü Hazırlama ve Slot Mekanikleri",
            Slug: uniqueSlug,
            TopicCluster: "FrpDnD",
            TldrSummary: "Dungeons and Dragons beşinci edisyonda büyü hazırlama, cantrip kullanımı, büyü slotlarının yenilenmesi ve spellcasting bileşenleri hakkında kapsamlı oyuncu rehberi ve ipuçları.",
            MetaDescription: "D&D 5E büyü mekanikleri rehberi. Büyücü, Ruhban ve Druid sınıflarının büyü hazırlama kuralları.",
            ContentMarkdown: "## Büyü Mekanikleri\nBüyü slotları uzun dinlenme (Long Rest) ile yenilenir. Cantripler ise limitsizce kullanılabilir.",
            AuthorName: "Büyü Ustası",
            FaqJson: null,
            CoverImageUrl: null);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/admin/articles", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var articleId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        articleId.Should().NotBeEmpty();

        // 2. Update Article
        var updateDto = createDto with
        {
            Title = "D&D 5E Büyü Mekanikleri Güncel Genişletilmiş Kılavuz"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/admin/articles/{articleId}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Delete Article (Soft Delete)
        var deleteResponse = await _client.DeleteAsync($"/api/v1/admin/articles/{articleId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 4. Silinen makale public uçta bulunamaz (404)
        var deletedCheck = await _client.GetAsync($"/api/v1/public/articles/{uniqueSlug}");
        deletedCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Snapshot Export All

    [Fact]
    public async Task AdminSnapshot_ExportAll_ShouldReturn200Ok()
    {
        // Act
        var response = await _client.PostAsync("/api/v1/admin/snapshot/export-all", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
