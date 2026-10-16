using FluentValidation;
using Markdig;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.Interfaces;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masabasi.Business.Services;

/// <summary>
/// Masabaşı Topic Cluster SEO blog, rehber ve kural makalelerini yöneten servis.
/// Markdown içerikleri Markdig motoru ile güvenli HTML'e dönüştürür.
/// </summary>
public sealed class ArticleManager : IArticleService
{
    private readonly MasabasiDbContext _dbContext;
    private readonly IStaticSnapshotService _snapshotService;
    private readonly IValidator<CreateArticleDto> _articleValidator;
    private readonly ILogger<ArticleManager> _logger;

    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAutoLinks()
        .UsePipeTables()
        .UseEmphasisExtras()
        .Build();

    public ArticleManager(
        MasabasiDbContext dbContext,
        IStaticSnapshotService snapshotService,
        IValidator<CreateArticleDto> articleValidator,
        ILogger<ArticleManager> logger)
    {
        _dbContext = dbContext;
        _snapshotService = snapshotService;
        _articleValidator = articleValidator;
        _logger = logger;
    }

    public async Task<ServiceResult<PagedResult<ArticleSummaryDto>>> GetArticlesAsync(
        string? topicCluster = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = _dbContext.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.IsPublished);

        if (!string.IsNullOrWhiteSpace(topicCluster))
        {
            query = query.Where(a => a.TopicCluster.ToLower() == topicCluster.Trim().ToLower());
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.PublishedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ArticleSummaryDto(
                a.Id,
                a.Title,
                a.Slug,
                a.TopicCluster,
                a.TldrSummary,
                a.MetaDescription,
                a.AuthorName,
                a.PublishedAtUtc,
                a.EstimatedReadTimeMinutes,
                a.CoverImageUrl))
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<ArticleSummaryDto>(items, totalCount, pageNumber, pageSize);
        return ServiceResult<PagedResult<ArticleSummaryDto>>.Success(pagedResult);
    }

    public async Task<ServiceResult<ArticleDetailDto>> GetArticleBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.Articles
            .FirstOrDefaultAsync(a => a.Slug == slug && !a.IsDeleted && a.IsPublished, cancellationToken);

        if (article is null)
            return ServiceResult<ArticleDetailDto>.Failure("Makale bulunamadı.", 404);

        // Görüntülenme sayısını artır
        article.ViewCount += 1;
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Markdown -> HTML Dönüşümü
        string htmlContent = Markdown.ToHtml(article.ContentMarkdown, MarkdownPipeline);

        var detail = new ArticleDetailDto(
            article.Id,
            article.Title,
            article.Slug,
            article.TopicCluster,
            article.TldrSummary,
            article.MetaDescription,
            article.ContentMarkdown,
            htmlContent,
            article.AuthorName,
            article.PublishedAtUtc,
            article.EstimatedReadTimeMinutes,
            article.FaqJson,
            article.CoverImageUrl,
            article.ViewCount);

        return ServiceResult<ArticleDetailDto>.Success(detail);
    }

    public async Task<ServiceResult<Guid>> CreateArticleAsync(
        CreateArticleDto request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _articleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            string error = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return ServiceResult<Guid>.Failure(error, 400);
        }

        bool slugExists = await _dbContext.Articles.AnyAsync(a => a.Slug == request.Slug, cancellationToken);
        if (slugExists)
            return ServiceResult<Guid>.Failure("Bu SEO kısa adına (slug) sahip bir makale zaten mevcut.", 409);

        int wordCount = request.ContentMarkdown
            .Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
        int estimatedMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / 180.0));

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Slug = request.Slug.Trim().ToLowerInvariant(),
            TopicCluster = request.TopicCluster.Trim(),
            TldrSummary = request.TldrSummary.Trim(),
            MetaDescription = request.MetaDescription.Trim(),
            ContentMarkdown = request.ContentMarkdown.Trim(),
            AuthorName = request.AuthorName.Trim(),
            FaqJson = request.FaqJson?.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim(),
            EstimatedReadTimeMinutes = estimatedMinutes,
            IsPublished = true,
            PublishedAtUtc = DateTimeOffset.UtcNow,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _dbContext.Articles.Add(article);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _snapshotService.ExportArticlesSnapshotAsync(cancellationToken);
        await _snapshotService.ExportSitemapXmlAsync(cancellationToken);

        _logger.LogInformation("Yeni makale yayınlandı: {Title} ({Id})", article.Title, article.Id);
        return ServiceResult<Guid>.Success(article.Id, 201);
    }

    public async Task<ServiceResult<bool>> UpdateArticleAsync(
        Guid id,
        CreateArticleDto request,
        CancellationToken cancellationToken = default)
    {
        var article = await _dbContext.Articles.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
        if (article is null)
            return ServiceResult<bool>.Failure("Güncellenecek makale bulunamadı.", 404);

        var validation = await _articleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            string error = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return ServiceResult<bool>.Failure(error, 400);
        }

        int wordCount = request.ContentMarkdown
            .Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;

        article.Title = request.Title.Trim();
        article.TopicCluster = request.TopicCluster.Trim();
        article.TldrSummary = request.TldrSummary.Trim();
        article.MetaDescription = request.MetaDescription.Trim();
        article.ContentMarkdown = request.ContentMarkdown.Trim();
        article.AuthorName = request.AuthorName.Trim();
        article.FaqJson = request.FaqJson?.Trim();
        article.CoverImageUrl = request.CoverImageUrl?.Trim();
        article.EstimatedReadTimeMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / 180.0));

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _snapshotService.ExportArticlesSnapshotAsync(cancellationToken);
        await _snapshotService.ExportSitemapXmlAsync(cancellationToken);

        _logger.LogInformation("Makale güncellendi: {Title}", article.Title);
        return ServiceResult<bool>.Success(true);
    }
}
