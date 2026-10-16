namespace Masabasi.Business.Common;

/// <summary>
/// Sayfalanmış liste yanıtları için jenerik veri taşıyıcı model.
/// </summary>
/// <typeparam name="T">Sayfadaki liste elemanlarının tipi.</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    /// <summary>Toplam sayfa sayısı.</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>Sonraki sayfa mevcut mu?</summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>Önceki sayfa mevcut mu?</summary>
    public bool HasPreviousPage => PageNumber > 1;
}
