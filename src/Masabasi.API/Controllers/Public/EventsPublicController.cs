using Masabasi.API.Extensions;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Controllers.Public;

/// <summary>
/// Halka açık Cumartesi etkinlikleri ve masa doluluk uçlarını sunan controller.
/// OutputCache ile RAM'den yanıt vererek yüksek trafikte veritabanını korur.
/// </summary>
[Route("api/v1/public/events")]
[EnableRateLimiting(RateLimitingExtensions.PublicReadPolicy)]
public sealed class EventsPublicController : BaseApiController
{
    private readonly IEventService _eventService;

    public EventsPublicController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Yaklaşan aktif Cumartesi etkinliklerini ve oyun masalarını listeler.
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = 30)]
    public async Task<IActionResult> GetActiveEvents(CancellationToken cancellationToken)
    {
        var result = await _eventService.GetActiveEventsCachedAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// SEO slug bilgisine göre tek bir etkinliği ve masalarını getirir.
    /// </summary>
    [HttpGet("{slug}")]
    [OutputCache(Duration = 30)]
    public async Task<IActionResult> GetEventBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await _eventService.GetEventBySlugAsync(slug, cancellationToken);
        return HandleResult(result);
    }
}
