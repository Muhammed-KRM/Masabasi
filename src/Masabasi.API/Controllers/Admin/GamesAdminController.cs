using Masabasi.API.Extensions;
using Masabasi.Business.DTOs.Games;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Controllers.Admin;

/// <summary>
/// Kutu oyunu ve FRP kataloğu yönetimi uçları.
/// </summary>
[Route("api/v1/admin/games")]
[EnableRateLimiting(RateLimitingExtensions.AdminIsolatedPolicy)]
public sealed class GamesAdminController : BaseApiController
{
    private readonly IGameCatalogService _gameCatalogService;

    public GamesAdminController(IGameCatalogService gameCatalogService)
    {
        _gameCatalogService = gameCatalogService;
    }

    /// <summary>
    /// Kütüphaneye yeni bir oyun veya FRP kural seti ekler.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateBoardGameDto request, CancellationToken cancellationToken)
    {
        var result = await _gameCatalogService.CreateGameAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mevcut bir oyunun kural rehberini veya envanter bilgilerini günceller.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGame(Guid id, [FromBody] CreateBoardGameDto request, CancellationToken cancellationToken)
    {
        var result = await _gameCatalogService.UpdateGameAsync(id, request, cancellationToken);
        return HandleResult(result);
    }
}
