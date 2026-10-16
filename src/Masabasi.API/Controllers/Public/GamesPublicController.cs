using Masabasi.API.Extensions;
using Masabasi.Business.DTOs.Games;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Controllers.Public;

/// <summary>
/// Kulüp kutu oyunu ve FRP kütüphanesini halka açık olarak listeleyen controller.
/// </summary>
[Route("api/v1/public/games")]
[EnableRateLimiting(RateLimitingExtensions.PublicReadPolicy)]
public sealed class GamesPublicController : BaseApiController
{
    private readonly IGameCatalogService _gameCatalogService;

    public GamesPublicController(IGameCatalogService gameCatalogService)
    {
        _gameCatalogService = gameCatalogService;
    }

    /// <summary>
    /// Kategori, oyuncu sayısı ve karmaşıklığa göre filtrelenmiş oyun kataloğunu getirir.
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = 60, Tags = ["games"])]
    public async Task<IActionResult> GetGames([FromQuery] GameFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _gameCatalogService.GetGamesAsync(filter, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// SEO slug bilgisine göre oyunun kural rehberini ve detaylarını getirir.
    /// </summary>
    [HttpGet("{slug}")]
    [OutputCache(Duration = 60, Tags = ["games"])]
    public async Task<IActionResult> GetGameBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await _gameCatalogService.GetGameBySlugAsync(slug, cancellationToken);
        return HandleResult(result);
    }
}
