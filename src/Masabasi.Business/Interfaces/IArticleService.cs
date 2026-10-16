using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Articles;

namespace Masabasi.Business.Interfaces;

/// <summary>
/// Masabaşı SEO ve Topic Cluster rehberleri, kural yazıları ve geek kültür blog içeriklerini yöneten servis sözleşmesi.
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// Kategoriye göre filtrelenmiş ve sayfalanmış blog/rehber listesini getirir.
    /// </summary>
    Task<ServiceResult<PagedResult<ArticleSummaryDto>>> GetArticlesAsync(
        string? topicCluster = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen SEO slug değerine göre makale detayını ve Markdown'dan derlenmiş HTML içeriğini getirir.
    /// </summary>
    Task<ServiceResult<ArticleDetailDto>> GetArticleBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Yeni SEO uyumlu makale yayınlar, okuma süresini otomatik hesaplar ve snapshot güncellemesi yapar.
    /// </summary>
    Task<ServiceResult<Guid>> CreateArticleAsync(CreateArticleDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut bir makaleyi günceller.
    /// </summary>
    Task<ServiceResult<bool>> UpdateArticleAsync(Guid id, CreateArticleDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir makaleyi mantıksal olarak siler (soft delete) ve snapshot'ları günceller.
    /// </summary>
    Task<ServiceResult<bool>> DeleteArticleAsync(Guid id, CancellationToken cancellationToken = default);
}
