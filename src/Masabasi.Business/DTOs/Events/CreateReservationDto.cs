namespace Masabasi.Business.DTOs.Events;

/// <summary>
/// Oyuncunun seçtiği masaya koltuk ayırtması için API'ye gönderdiği istek modeli.
/// </summary>
public sealed record CreateReservationDto(
    Guid GameTableId,
    string PlayerName,
    string ContactHandle,
    string ExperienceLevel,
    string? Notes);
