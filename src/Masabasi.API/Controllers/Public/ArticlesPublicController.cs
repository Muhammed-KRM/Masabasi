using Masabasi.API.Extensions;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Controllers.Public;

/// <summary>
/// Masabaşı Topic Cluster SEO blog, kural rehberleri ve geek kültür içeriklerini sunan controller.
/// </summary>
[Route("api/v1/public/articles")]
[EnableRateLimiting(RateLimitingExtensions.PublicReadPolicy)]
public sealed class ArticlesPublicController : BaseApiController
{
    private readonly IArticleService _articleService;

    public ArticlesPublicController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// Kategoriye göre filtrelenmiş ve sayfalanmış blog makalelerini listeler.
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetArticles(
        [FromQuery] string? topicCluster,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _articleService.GetArticlesAsync(topicCluster, pageNumber, pageSize, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// SEO slug bilgisine göre makale detayını ve Markdown'dan derlenmiş HTML içeriğini getirir.
    /// </summary>
    [HttpGet("{slug}")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetArticleBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await _articleService.GetArticleBySlugAsync(slug, cancellationToken);
        return HandleResult(result);
    }
}
