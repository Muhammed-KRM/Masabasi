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
/// ArticleManager servisinin Markdown -> HTML derlemesi, SEO doğrulamaları ve okuma süresi hesaplamalarını doğrulayan testler.
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

    [Fact]
    public async Task CreateArticleAsync_ShouldValidateAndCalculateEstimatedReadTime()
    {
        // Arrange
        var (_, manager, snapshotMock) = CreateTestFixture();

        // 360 kelimelik içerik (~2 dakika okuma süresi)
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

        // Act
        var result = await manager.CreateArticleAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        var articleDetail = await manager.GetArticleBySlugAsync("dnd-5e-karakter-kagidi-olusturma-kilavuzu");
        articleDetail.IsSuccess.Should().BeTrue();
        articleDetail.Data!.EstimatedReadTimeMinutes.Should().BeGreaterThanOrEqualTo(2);
        articleDetail.Data.ContentHtml.Should().Contain("<h2>Başlık</h2>");

        snapshotMock.Verify(s => s.ExportArticlesSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetArticleBySlugAsync_ShouldIncrementViewCount()
    {
        // Arrange
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

        // Act
        var view1 = await manager.GetArticleBySlugAsync("secret-hitler-taktikleri-liberaller-icin-kilavuz");
        var view2 = await manager.GetArticleBySlugAsync("secret-hitler-taktikleri-liberaller-icin-kilavuz");

        // Assert
        view1.Data!.ViewCount.Should().Be(1);
        view2.Data!.ViewCount.Should().Be(2);
    }
}
