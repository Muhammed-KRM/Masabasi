using Masabasi.API.Extensions;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Controllers.Admin;

/// <summary>
/// Topic Cluster SEO blog ve kural rehberleri yönetim uçları.
/// </summary>
[Route("api/v1/admin/articles")]
[EnableRateLimiting(RateLimitingExtensions.AdminIsolatedPolicy)]
public sealed class ArticlesAdminController : BaseApiController
{
    private readonly IArticleService _articleService;

    public ArticlesAdminController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// Yeni bir SEO makalesi yayınlar.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto request, CancellationToken cancellationToken)
    {
        var result = await _articleService.CreateArticleAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mevcut bir makaleyi günceller.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] CreateArticleDto request, CancellationToken cancellationToken)
    {
        var result = await _articleService.UpdateArticleAsync(id, request, cancellationToken);
        return HandleResult(result);
    }
}
