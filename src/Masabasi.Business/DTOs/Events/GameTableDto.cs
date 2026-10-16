namespace Masabasi.Business.DTOs.Events;

/// <summary>
/// Bir Cumartesi etkinliğinde açılan oyun masası ve doluluk durumunu temsil eden DTO.
/// </summary>
public sealed record GameTableDto(
    Guid Id,
    int TableNumber,
    string GameTitle,
    string Category,
    string HostOrDmName,
    int MaxPlayers,
    int CurrentPlayers,
    bool IsBeginnerFriendly)
{
    /// <summary>Kalan boş koltuk sayısı.</summary>
    public int RemainingSeats => Math.Max(0, MaxPlayers - CurrentPlayers);

    /// <summary>Masa kontenjanı tamamen doldu mu?</summary>
    public bool IsFull => RemainingSeats == 0;
}
