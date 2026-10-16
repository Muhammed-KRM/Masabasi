namespace Masabasi.Business.DTOs.Events;

/// <summary>
/// Başarılı masa koltuk rezervasyonu sonrasında dönen sonuç modeli.
/// </summary>
public sealed record ReservationResultDto(
    Guid ReservationId,
    Guid TableId,
    int TableNumber,
    string GameTitle,
    int CurrentPlayers,
    int MaxPlayers);
