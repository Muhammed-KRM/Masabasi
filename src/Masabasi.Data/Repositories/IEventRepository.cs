using Masabasi.Data.Entities;

namespace Masabasi.Data.Repositories;

/// <summary>
/// Cumartesi etkinlikleri ve ilişkili masaların optimize sorgularını tanımlayan özel depo sözleşmesi.
/// </summary>
public interface IEventRepository : IGenericRepository<ClubEvent>
{
    /// <summary>
    /// Yaklaşan aktif Cumartesi etkinliklerini, içlerindeki oyun masaları ve doluluk durumlarıyla birlikte getirir.
    /// </summary>
    Task<IReadOnlyList<ClubEvent>> GetUpcomingEventsWithTablesAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen SEO slug değerine sahip etkinliği tüm masaları ve onaylı rezervasyonlarıyla birlikte getirir.
    /// </summary>
    Task<ClubEvent?> GetEventWithTablesBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen kimliğe sahip etkinliği tüm masaları ve onaylı rezervasyonlarıyla birlikte getirir.
    /// </summary>
    Task<ClubEvent?> GetEventWithTablesByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tüm geçmiş ve güncel etkinlikleri masalarıyla birlikte sıralı getirir.
    /// </summary>
    Task<IReadOnlyList<ClubEvent>> GetAllEventsWithTablesAsync(CancellationToken cancellationToken = default);
}
