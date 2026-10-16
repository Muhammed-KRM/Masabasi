namespace Masabasi.Business.DTOs.Games;

/// <summary>
/// Kutu oyunu ve FRP kütüphanesindeki oyun detaylarını taşıyan DTO.
/// </summary>
public sealed record BoardGameDto(
    Guid Id,
    string Title,
    string Slug,
    string Category,
    int MinPlayers,
    int MaxPlayers,
    string? RecommendedPlayerCount,
    int PlayTimeMinutes,
    double ComplexityRating,
    string TldrSummary,
    string RulesMarkdown,
    string? ImageUrl,
    bool IsInClubInventory,
    int? YearPublished);

/// <summary>
/// Yeni kutu oyunu veya FRP kural rehberi ekleme modeli.
/// </summary>
public sealed record CreateBoardGameDto(
    string Title,
    string Slug,
    string Category,
    int MinPlayers,
    int MaxPlayers,
    string? RecommendedPlayerCount,
    int PlayTimeMinutes,
    double ComplexityRating,
    string TldrSummary,
    string RulesMarkdown,
    string? ImageUrl,
    bool IsInClubInventory,
    int? YearPublished);

/// <summary>
/// Oyun kütüphanesini filtreleme parametreleri.
/// </summary>
public sealed record GameFilterDto(
    string? Category = null,
    int? PlayerCount = null,
    double? MaxComplexity = null,
    string? SearchTerm = null,
    bool? OnlyInInventory = null);
