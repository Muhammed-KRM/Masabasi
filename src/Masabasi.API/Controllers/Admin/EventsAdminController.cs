using Masabasi.API.Extensions;
using Masabasi.API.Hubs;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;

namespace Masabasi.API.Controllers.Admin;

/// <summary>
/// Admin ve Oyun Yöneticileri (DM) için Cumartesi etkinlik ve masa yönetim uçları.
/// Vitrin trafiğinden tamamen izole hız sınırı (AdminIsolatedPolicy) altındadır.
/// </summary>
[Route("api/v1/admin/events")]
[EnableRateLimiting(RateLimitingExtensions.AdminIsolatedPolicy)]
public sealed class EventsAdminController : BaseApiController
{
    private readonly IEventService _eventService;
    private readonly IHubContext<TableHub> _tableHubContext;

    public EventsAdminController(IEventService eventService, IHubContext<TableHub> tableHubContext)
    {
        _eventService = eventService;
        _tableHubContext = tableHubContext;
    }

    /// <summary>
    /// Yeni bir Cumartesi buluşması oluşturur.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto request, CancellationToken cancellationToken)
    {
        var result = await _eventService.CreateEventAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mevcut bir etkinliğe yeni bir oyun veya FRP masası ekler.
    /// </summary>
    [HttpPost("tables")]
    public async Task<IActionResult> AddTable([FromBody] CreateGameTableDto request, CancellationToken cancellationToken)
    {
        var result = await _eventService.AddTableAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Bir masadaki oyuncu rezervasyonunu iptal eder.
    /// </summary>
    [HttpDelete("reservations/{id:guid}")]
    public async Task<IActionResult> CancelReservation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _eventService.CancelReservationAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
