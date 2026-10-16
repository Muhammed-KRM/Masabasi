using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Events;

namespace Masabasi.Business.Interfaces;

/// <summary>
/// Cumartesi etkinlikleri, oyun masaları ve koltuk rezervasyonu iş mantığını yöneten servis sözleşmesi.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Vitrin için yaklaşan aktif Cumartesi etkinliklerini HybridCache önbelleğinden getirir.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<EventDetailDto>>> GetActiveEventsCachedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilen SEO slug değerine göre etkinlik detayını masalarıyla birlikte getirir.
    /// </summary>
    Task<ServiceResult<EventDetailDto>> GetEventBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kimliğe göre etkinlik detayını getirir.
    /// </summary>
    Task<ServiceResult<EventDetailDto>> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir masaya yeni bir oyuncu kaydı ekler. Optimistic concurrency ile overbooking koruması sağlar.
    /// </summary>
    Task<ServiceResult<ReservationResultDto>> ReserveTableSeatAsync(CreateReservationDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin panelinden yeni bir Cumartesi etkinliği oluşturur.
    /// </summary>
    Task<ServiceResult<Guid>> CreateEventAsync(CreateEventDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut bir etkinliğe yeni oyun veya FRP masası ekler.
    /// </summary>
    Task<ServiceResult<Guid>> AddTableAsync(CreateGameTableDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir rezervasyonu iptal eder ve masadaki doluluk sayısını düşürür.
    /// </summary>
    Task<ServiceResult<bool>> CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
}
