using FluentAssertions;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.Interfaces;
using Masabasi.Business.Services;
using Masabasi.Business.Validators;
using Masabasi.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// ArticleManager servisinin Markdown -> HTML derlemesi, SEO doğrulamaları, sayfalama ve okuma süresi hesaplamalarını doğrulayan testler.
/// </summary>
public sealed class ArticleManagerTests
{
    private static (MasabasiDbContext Db, ArticleManager Manager, Mock<IStaticSnapshotService> SnapshotMock) CreateTestFixture()
    {
        var dbOptions = new DbContextOptionsBuilder<MasabasiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var db = new MasabasiDbContext(dbOptions);
        var snapshotMock = new Mock<IStaticSnapshotService>();
        var validator = new CreateArticleValidator();
        var logger = NullLogger<ArticleManager>.Instance;

        var manager = new ArticleManager(db, snapshotMock.Object, validator, logger);
        return (db, manager, snapshotMock);
    }

    private static CreateArticleDto CreateValidDto(string title, string slug, string cluster) => new(
        Title: title,
        Slug: slug,
        TopicCluster: cluster,
        TldrSummary: "Masabaşı Oyun Kulübü tarafından hazırlanan bu kapsamlı rehber makalesinde en temel kutu oyunu kuralları, masa stratejileri ve oyuncu taktikleri detaylıca anlatılmaktadır.",
        MetaDescription: "Masabaşı Oyun Kulübü kutu oyunları rehberi ile stratejileri, kuralları ve en popüler masa oyunlarını hemen keşfedin.",
        ContentMarkdown: "## Başlık\n" + string.Join(" ", Enumerable.Repeat("detaylı içerik kelimesi", 50)),
        AuthorName: "Yazar",
        FaqJson: null,
        CoverImageUrl: null);

    [Fact]
    public async Task CreateArticleAsync_ShouldValidateAndCalculateEstimatedReadTime()
    {
        var (_, manager, snapshotMock) = CreateTestFixture();

        string words = string.Join(" ", Enumerable.Repeat("oyun", 360));
        string markdownContent = $"## Başlık\n\n{words}";

        var request = new CreateArticleDto(
            Title: "D&D 5e Karakter Kağıdı Oluşturma Kılavuzu",
            Slug: "dnd-5e-karakter-kagidi-olusturma-kilavuzu",
            TopicCluster: "FrpDnD",
            TldrSummary: "D&D 5e oyununa başlarken karakter kağıdınızı kolayca doldurmanız için gereken tüm adımlar ve Türkçe açıklamaları burada bulabilirsiniz. Masabaşı Oyun Kulübü ile maceraya ilk adımınızı güvenle atın.",
            MetaDescription: "D&D 5e karakter kağıdı nasıl doldurulur? Karakter yaratma adımları, stat puanları ve yetenek seçimleri detaylı anlatımı Masabaşı'nda.",
            ContentMarkdown: markdownContent,
            AuthorName: "DM Efe",
            FaqJson: null,
            CoverImageUrl: null);

        var result = await manager.CreateArticleAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        var articleDetail = await manager.GetArticleBySlugAsync("dnd-5e-karakter-kagidi-olusturma-kilavuzu");
        articleDetail.IsSuccess.Should().BeTrue();
        articleDetail.Data!.EstimatedReadTimeMinutes.Should().BeGreaterThanOrEqualTo(2);
        articleDetail.Data.ContentHtml.Should().Contain("<h2>Başlık</h2>");

        snapshotMock.Verify(s => s.ExportArticlesSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateArticleAsync_ShouldFail_OnDuplicateSlug()
    {
        var (_, manager, _) = CreateTestFixture();

        var dto = CreateValidDto("İlk Özgün Makale Başlığı Yeterli", "ayni-slug", "KutuOyunlari");
        var result1 = await manager.CreateArticleAsync(dto);
        result1.IsSuccess.Should().BeTrue();

        var result2 = await manager.CreateArticleAsync(dto);
        result2.IsSuccess.Should().BeFalse();
        result2.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task GetArticleBySlugAsync_ShouldIncrementViewCount()
    {
        var (_, manager, _) = CreateTestFixture();

        var request = new CreateArticleDto(
            Title: "Secret Hitler Taktikleri: Liberaller İçin Kılavuz",
            Slug: "secret-hitler-taktikleri-liberaller-icin-kilavuz",
            TopicCluster: "KutuOyunlari",
            TldrSummary: "Secret Hitler oyununda liberallerin kazanma stratejileri, faşistlerin blöflerini çözme taktikleri ve dikkat edilmesi gereken kart olasılıkları. Masada güven ortamı kurmanın tüm inceliklerini bu rehberde bulabilirsiniz.",
            MetaDescription: "Secret Hitler oyununda liberal olarak kazanma taktikleri. Şansölye seçimleri ve politika güven testleri hakkında bilgiler.",
            ContentMarkdown: "## Taktikler\nKartları dikkatle takip edin ve güven zincirleri kurun. Liberaller daima şeffaf bilgi paylaşmalı, faşistlerin kafa karıştırma çabalarına karşı dikkatli olmalıdır.",
            AuthorName: "Masabaşı Ekibi",
            FaqJson: null,
            CoverImageUrl: null);

        await manager.CreateArticleAsync(request);

        var view1 = await manager.GetArticleBySlugAsync("secret-hitler-taktikleri-liberaller-icin-kilavuz");
        var view2 = await manager.GetArticleBySlugAsync("secret-hitler-taktikleri-liberaller-icin-kilavuz");

        view1.Data!.ViewCount.Should().Be(1);
        view2.Data!.ViewCount.Should().Be(2);

        var notFound = await manager.GetArticleBySlugAsync("olmayan-makale");
        notFound.IsSuccess.Should().BeFalse();
        notFound.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetArticlesAsync_ShouldFilterByCluster_AndPaginate()
    {
        var (_, manager, _) = CreateTestFixture();

        await manager.CreateArticleAsync(CreateValidDto("Makale 1 Başlığı Yeterli Uzunluk", "makale-1", "FrpDnD"));
        await manager.CreateArticleAsync(CreateValidDto("Makale 2 Başlığı Yeterli Uzunluk", "makale-2", "FrpDnD"));
        await manager.CreateArticleAsync(CreateValidDto("Makale 3 Başlığı Yeterli Uzunluk", "makale-3", "KutuOyunlari"));

        // Kategori filtresi
        var frpList = await manager.GetArticlesAsync(topicCluster: "FrpDnD", pageNumber: 1, pageSize: 10);
        frpList.IsSuccess.Should().BeTrue();
        frpList.Data!.TotalCount.Should().Be(2);
        frpList.Data.Items.Should().OnlyContain(a => a.TopicCluster == "FrpDnD");

        // Sayfalama (sayfa boyutu 1)
        var paged = await manager.GetArticlesAsync(pageNumber: 1, pageSize: 1);
        paged.Data!.Items.Should().HaveCount(1);
        paged.Data.TotalCount.Should().Be(3);
        paged.Data.TotalPages.Should().Be(3);
        paged.Data.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateArticleAsync_ShouldUpdateContent_AndReturnNotFoundOnMissing()
    {
        var (_, manager, snapshotMock) = CreateTestFixture();

        var initial = CreateValidDto("İlk Başlık Uzunluğu Yeterli Seviyede", "guncellenecek-slug", "KutuOyunlari");
        var createResult = await manager.CreateArticleAsync(initial);
        createResult.IsSuccess.Should().BeTrue();

        var updateDto = initial with
        {
            Title = "Güncellenen Yeni Başlık Uzunluğu Yeterli",
            TopicCluster = "KartOyunlari"
        };

        var updateResult = await manager.UpdateArticleAsync(createResult.Data, updateDto);
        updateResult.IsSuccess.Should().BeTrue();

        var updated = await manager.GetArticleBySlugAsync("guncellenecek-slug");
        updated.Data!.Title.Should().Be("Güncellenen Yeni Başlık Uzunluğu Yeterli");
        updated.Data.TopicCluster.Should().Be("KartOyunlari");

        snapshotMock.Verify(s => s.ExportArticlesSnapshotAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));

        // Olmayan makale
        var notFound = await manager.UpdateArticleAsync(Guid.NewGuid(), updateDto);
        notFound.IsSuccess.Should().BeFalse();
        notFound.StatusCode.Should().Be(404);
    }
}
