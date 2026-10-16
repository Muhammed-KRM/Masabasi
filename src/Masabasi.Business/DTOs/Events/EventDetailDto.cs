namespace Masabasi.Business.DTOs.Events;

/// <summary>
/// Cumartesi etkinliğinin vitrin ve API yanıtlarında kullanılan kapsamlı detay modeli.
/// </summary>
public sealed record EventDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string TldrSummary,
    DateTimeOffset StartDateUtc,
    DateTimeOffset EndDateUtc,
    string VenueName,
    string VenueAddress,
    string? MeetupEventUrl,
    string? ExternalRegistrationUrl,
    IReadOnlyList<GameTableDto> Tables);
