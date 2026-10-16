using Masabasi.API.Extensions;
using Masabasi.API.Hubs;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;

namespace Masabasi.API.Controllers.Public;

/// <summary>
/// Masaya oyuncu koltuk kaydı alma ucu.
/// Spam bot korumalı (PublicReservationPolicy) ve SignalR canlı bildirim entegrasyonludur.
/// </summary>
[Route("api/v1/public/reservations")]
[EnableRateLimiting(RateLimitingExtensions.PublicReservationPolicy)]
public sealed class ReservationsPublicController : BaseApiController
{
    private readonly IEventService _eventService;
    private readonly IHubContext<TableHub> _tableHubContext;
    private readonly ILogger<ReservationsPublicController> _logger;

    public ReservationsPublicController(
        IEventService eventService,
        IHubContext<TableHub> tableHubContext,
        ILogger<ReservationsPublicController> logger)
    {
        _eventService = eventService;
        _tableHubContext = tableHubContext;
        _logger = logger;
    }

    /// <summary>
    /// Oyuncunun seçtiği masaya koltuk rezervasyonu oluşturur ve SignalR ile canlı kontenjan günceller.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReservation(
        [FromBody] CreateReservationDto request,
        CancellationToken cancellationToken)
    {
        var result = await _eventService.ReserveTableSeatAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            return HandleResult(result);
        }

        // SignalR üzerinden bağlı tüm vitrin ziyaretçilerine anlık masa güncellemesi gönder
        try
        {
            await _tableHubContext.Clients.All.SendAsync(
                "TableSeatUpdated",
                new
                {
                    tableId = result.Data!.TableId,
                    tableNumber = result.Data.TableNumber,
                    gameTitle = result.Data.GameTitle,
                    currentPlayers = result.Data.CurrentPlayers,
                    maxPlayers = result.Data.MaxPlayers
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR masa güncelleme yayını gönderilirken hata oluştu.");
        }

        return HandleResult(result);
    }
}
