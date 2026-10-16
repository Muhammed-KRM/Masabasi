using Microsoft.AspNetCore.SignalR;

namespace Masabasi.API.Hubs;

/// <summary>
/// Cumartesi oyun masalarındaki koltuk doluluk durumunu (Örn: "Masa 2: 4/5 Kişi")
/// bağlı tüm tarayıcılara eşzamanlı olarak yayınlayan SignalR Hub'ı.
/// </summary>
public sealed class TableHub : Hub
{
    private readonly ILogger<TableHub> _logger;

    public TableHub(ILogger<TableHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogDebug("SignalR istemcisi bağlandı: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("SignalR istemcisi ayrıldı: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// İstemcinin belirli bir Cumartesi etkinliğinin canlı oda yayınına katılmasını sağlar.
    /// </summary>
    public async Task JoinEventRoom(string eventSlug)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"event:{eventSlug.ToLowerInvariant()}");
        _logger.LogDebug("İstemci {ConnectionId} odaya katıldı: event:{EventSlug}", Context.ConnectionId, eventSlug);
    }

    /// <summary>
    /// İstemcinin etkinlik canlı odasından ayrılmasını sağlar.
    /// </summary>
    public async Task LeaveEventRoom(string eventSlug)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event:{eventSlug.ToLowerInvariant()}");
    }
}
