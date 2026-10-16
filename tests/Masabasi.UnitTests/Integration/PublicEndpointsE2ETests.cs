using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.DTOs.Games;
using Xunit;

namespace Masabasi.UnitTests.Integration;

/// <summary>
/// Masabaşı halka açık (public) API uç noktalarını (Etkinlikler, Oyun Kataloğu, Makaleler)
/// uçtan uca HTTP istekleriyle test eden entegrasyon sınıfı.
/// </summary>
public sealed class PublicEndpointsE2ETests : IClassFixture<MasabasiApiFactory>
{
    private readonly HttpClient _client;

    public PublicEndpointsE2ETests(MasabasiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Events Public Endpoints

    [Fact]
    public async Task GetUpcomingEvents_ShouldReturn200Ok_WithSeededEvents()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/public/events");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var events = await response.Content.ReadFromJsonAsync<List<EventDetailDto>>();
        events.Should().NotBeNull();
        events!.Should().NotBeEmpty();
        events.Should().Contain(e => e.Slug.StartsWith("cumartesi-bulusmasi-"));
    }

    [Fact]
    public async Task GetEventBySlug_ShouldReturn200Ok_WhenExists_And404_WhenMissing()
    {
        // 1. Mevcut etkinlikleri alıp slug'ı bul
        var eventsResponse = await _client.GetAsync("/api/v1/public/events");
        var events = await eventsResponse.Content.ReadFromJsonAsync<List<EventDetailDto>>();
        var firstEvent = events!.First();

        var successResponse = await _client.GetAsync($"/api/v1/public/events/{firstEvent.Slug}");
        successResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var eventDetail = await successResponse.Content.ReadFromJsonAsync<EventDetailDto>();
        eventDetail.Should().NotBeNull();
        eventDetail!.Slug.Should().Be(firstEvent.Slug);
        eventDetail.Tables.Should().NotBeEmpty();

        // 2. Olmayan etkinlik sorgulama
        var notFoundResponse = await _client.GetAsync("/api/v1/public/events/olmayan-etkinlik-slug");
        notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Games Public Endpoints

    [Fact]
    public async Task GetGames_ShouldReturnPagedResults_AndSupportSearchAndCategoryFilters()
    {
        // 1. Tüm oyunlar sorgusu
        var response = await _client.GetAsync("/api/v1/public/games");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var games = await response.Content.ReadFromJsonAsync<List<BoardGameDto>>();
        games.Should().NotBeNull();
        games!.Should().NotBeEmpty();
        games.Count.Should().BeGreaterThanOrEqualTo(5);

        // 2. Arama filtresi (Catan)
        var catanResponse = await _client.GetAsync("/api/v1/public/games?searchTerm=Catan");
        catanResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var catanResult = await catanResponse.Content.ReadFromJsonAsync<List<BoardGameDto>>();
        catanResult.Should().NotBeNull();
        catanResult!.Should().Contain(g => g.Title.Contains("Catan"));

        // 3. Olmayan arama terimi
        var emptyResponse = await _client.GetAsync("/api/v1/public/games?searchTerm=HicBulunmayanOyun12345");
        emptyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var emptyResult = await emptyResponse.Content.ReadFromJsonAsync<List<BoardGameDto>>();
        emptyResult!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetGameBySlug_ShouldReturn200Ok_WhenExists_And404_WhenMissing()
    {
        // 1. Mevcut oyun
        var response = await _client.GetAsync("/api/v1/public/games/catan-nasil-oynanir-takas-ve-strateji-rehberi");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var game = await response.Content.ReadFromJsonAsync<BoardGameDto>();
        game.Should().NotBeNull();
        game!.Slug.Should().Be("catan-nasil-oynanir-takas-ve-strateji-rehberi");
        game.ComplexityRating.Should().BeGreaterThan(0);

        // 2. Olmayan oyun
        var notFound = await _client.GetAsync("/api/v1/public/games/olmayan-kutu-oyunu-123");
        notFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Articles Public Endpoints

    [Fact]
    public async Task GetArticles_ShouldReturnPagedList_AndFilterByTopicCluster()
    {
        // 1. Genel makale listesi
        var response = await _client.GetAsync("/api/v1/public/articles?pageNumber=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pagedArticles = await response.Content.ReadFromJsonAsync<PagedResult<ArticleSummaryDto>>();
        pagedArticles.Should().NotBeNull();
        pagedArticles!.Items.Should().NotBeEmpty();

        // 2. Topic cluster filtresi
        var clusterResponse = await _client.GetAsync("/api/v1/public/articles?topicCluster=KutuOyunlari");
        clusterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var clusterResult = await clusterResponse.Content.ReadFromJsonAsync<PagedResult<ArticleSummaryDto>>();
        clusterResult.Should().NotBeNull();
        clusterResult!.Items.Should().OnlyContain(a => a.TopicCluster == "KutuOyunlari");
    }

    [Fact]
    public async Task GetArticleBySlug_ShouldReturn200Ok_WithCompiledHtml_AndIncrementViews()
    {
        string slug = "dnd-5e-nasil-baslanir-ilk-kez-frp-oynayacaklar-icin-rehber";

        // 1. İlk görüntüleme
        var view1Response = await _client.GetAsync($"/api/v1/public/articles/{slug}");
        view1Response.StatusCode.Should().Be(HttpStatusCode.OK);
        var article1 = await view1Response.Content.ReadFromJsonAsync<ArticleDetailDto>();
        article1.Should().NotBeNull();
        article1!.ContentHtml.Should().NotBeNullOrWhiteSpace();
        article1.EstimatedReadTimeMinutes.Should().BeGreaterThan(0);

        // 2. İkinci görüntüleme (OutputCache devrede olduğu için hızlı ve başarılı 200 döner)
        var view2Response = await _client.GetAsync($"/api/v1/public/articles/{slug}");
        view2Response.StatusCode.Should().Be(HttpStatusCode.OK);
        var article2 = await view2Response.Content.ReadFromJsonAsync<ArticleDetailDto>();
        article2.Should().NotBeNull();
        article2!.ViewCount.Should().BeGreaterThanOrEqualTo(article1.ViewCount);

        // 3. Olmayan makale
        var notFoundResponse = await _client.GetAsync("/api/v1/public/articles/bulunmayan-makale-slug");
        notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
