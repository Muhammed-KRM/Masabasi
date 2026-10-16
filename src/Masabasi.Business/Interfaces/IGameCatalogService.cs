using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Games;

namespace Masabasi.Business.Interfaces;

/// <summary>
/// Kulüp envanterindeki kutu oyunları, FRP sistemleri ve kural rehberleri iş mantığını yöneten servis sözleşmesi.
/// </summary>
public interface IGameCatalogService
{
    /// <summary>
    /// Kategori, oyuncu sayısı ve arama terimine göre filtrelenmiş oyun listesini getirir.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<BoardGameDto>>> GetGamesAsync(GameFilterDto filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen SEO slug değerine göre oyun kural rehberi ve detaylarını getirir.
    /// </summary>
    Task<ServiceResult<BoardGameDto>> GetGameBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Yeni kutu oyunu veya FRP sistemi ekler ve statik snapshot'ı tetikler.
    /// </summary>
    Task<ServiceResult<Guid>> CreateGameAsync(CreateBoardGameDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut bir oyunun kural rehberini veya bilgilerini günceller.
    /// </summary>
    Task<ServiceResult<bool>> UpdateGameAsync(Guid id, CreateBoardGameDto request, CancellationToken cancellationToken = default);
}
