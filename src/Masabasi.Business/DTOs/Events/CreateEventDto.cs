namespace Masabasi.Business.DTOs.Events;

/// <summary>
/// Admin panelinden yeni bir Cumartesi etkinliği açmak için gönderilen model.
/// </summary>
public sealed record CreateEventDto(
    string Title,
    string Slug,
    string TldrSummary,
    DateTimeOffset StartDateUtc,
    DateTimeOffset EndDateUtc,
    string VenueName,
    string VenueAddress,
    string? MeetupEventUrl,
    string? ExternalRegistrationUrl);

/// <summary>
/// Bir etkinliğe yeni masa eklemek için kullanılan DTO.
/// </summary>
public sealed record CreateGameTableDto(
    Guid ClubEventId,
    int TableNumber,
    string GameTitle,
    string Category,
    string HostOrDmName,
    int MaxPlayers,
    bool IsBeginnerFriendly);
