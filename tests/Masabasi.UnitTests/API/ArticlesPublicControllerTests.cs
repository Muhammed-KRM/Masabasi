using FluentAssertions;
using Masabasi.API.Controllers.Public;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// ArticlesPublicController blog ve makale halka açık endpoint testleri.
/// </summary>
public sealed class ArticlesPublicControllerTests
{
    private readonly Mock<IArticleService> _articleServiceMock = new();

    [Fact]
    public async Task GetArticles_ShouldReturnOk_WithPagedResult()
    {
        var summaries = new List<ArticleSummaryDto>
        {
            new(Guid.NewGuid(), "D&D Başlangıç", "dnd-baslangic", "FrpDnD", "Özet", "Meta", "Yazar", DateTimeOffset.UtcNow, 5, null)
        };
        var paged = new PagedResult<ArticleSummaryDto>(summaries, 1, 1, 10);

        _articleServiceMock
            .Setup(s => s.GetArticlesAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<PagedResult<ArticleSummaryDto>>.Success(paged));

        var controller = new ArticlesPublicController(_articleServiceMock.Object);

        var response = await controller.GetArticles(null, 1, 10, CancellationToken.None);

        var ok = response as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var data = ok.Value as PagedResult<ArticleSummaryDto>;
        data.Should().NotBeNull();
        data!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetArticleBySlug_ShouldReturnArticleDetailOrNotFound()
    {
        var detail = new ArticleDetailDto(
            Guid.NewGuid(), "Secret Hitler", "secret-hitler", "KutuOyunlari",
            "Özet", "Meta", "## Markdown", "<p>HTML</p>", "Yazar",
            DateTimeOffset.UtcNow, 4, null, null, 10);

        _articleServiceMock
            .Setup(s => s.GetArticleBySlugAsync("secret-hitler", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<ArticleDetailDto>.Success(detail));

        _articleServiceMock
            .Setup(s => s.GetArticleBySlugAsync("olmayan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<ArticleDetailDto>.Failure("Makale bulunamadı.", 404));

        var controller = new ArticlesPublicController(_articleServiceMock.Object);

        var found = await controller.GetArticleBySlug("secret-hitler", CancellationToken.None) as OkObjectResult;
        found.Should().NotBeNull();
        found!.StatusCode.Should().Be(200);

        var notFound = await controller.GetArticleBySlug("olmayan", CancellationToken.None) as ObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(404);
    }
}
