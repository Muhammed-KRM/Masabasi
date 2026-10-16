using FluentValidation;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Interfaces;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Masabasi.Business.Services;

/// <summary>
/// Cumartesi etkinliklerinin, oyun masalarının ve oyuncu koltuk kayıtlarının iş kurallarını yöneten servis.
/// </summary>
public sealed class EventManager : IEventService
{
    private readonly MasabasiDbContext _dbContext;
    private readonly IStaticSnapshotService _snapshotService;
    private readonly HybridCache _hybridCache;
    private readonly IValidator<CreateReservationDto> _reservationValidator;
    private readonly IValidator<CreateEventDto> _eventValidator;
    private readonly ILogger<EventManager> _logger;

    private const string ActiveEventsCacheKey = "public:events:active-list";

    public EventManager(
        MasabasiDbContext dbContext,
        IStaticSnapshotService snapshotService,
        HybridCache hybridCache,
        IValidator<CreateReservationDto> reservationValidator,
        IValidator<CreateEventDto> eventValidator,
        ILogger<EventManager> logger)
    {
        _dbContext = dbContext;
        _snapshotService = snapshotService;
        _hybridCache = hybridCache;
        _reservationValidator = reservationValidator;
        _eventValidator = eventValidator;
        _logger = logger;
    }

    public async Task<ServiceResult<IReadOnlyList<EventDetailDto>>> GetActiveEventsCachedAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var events = await _hybridCache.GetOrCreateAsync(
                ActiveEventsCacheKey,
                async cancel => await FetchActiveEventsFromDbAsync(cancel),
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(5),
                    LocalCacheExpiration = TimeSpan.FromSeconds(30)
                },
                cancellationToken: cancellationToken);

            return ServiceResult<IReadOnlyList<EventDetailDto>>.Success(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aktif etkinlikler önbellekten getirilirken hata oluştu.");
            // Önbellek hatası olursa doğrudan veritabanından getir
            var fallback = await FetchActiveEventsFromDbAsync(cancellationToken);
            return ServiceResult<IReadOnlyList<EventDetailDto>>.Success(fallback);
        }
    }

    public async Task<ServiceResult<EventDetailDto>> GetEventBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var ev = await _dbContext.ClubEvents
            .AsNoTracking()
            .Include(e => e.Tables.Where(t => !t.IsDeleted).OrderBy(t => t.TableNumber))
            .FirstOrDefaultAsync(e => e.Slug == slug && !e.IsDeleted, cancellationToken);

        if (ev is null)
            return ServiceResult<EventDetailDto>.Failure("Etkinlik bulunamadı.", 404);

        return ServiceResult<EventDetailDto>.Success(MapToEventDetailDto(ev));
    }

    public async Task<ServiceResult<EventDetailDto>> GetEventByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var ev = await _dbContext.ClubEvents
            .AsNoTracking()
            .Include(e => e.Tables.Where(t => !t.IsDeleted).OrderBy(t => t.TableNumber))
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

        if (ev is null)
            return ServiceResult<EventDetailDto>.Failure("Etkinlik bulunamadı.", 404);

        return ServiceResult<EventDetailDto>.Success(MapToEventDetailDto(ev));
    }

    public async Task<ServiceResult<ReservationResultDto>> ReserveTableSeatAsync(
        CreateReservationDto request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _reservationValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            string error = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return ServiceResult<ReservationResultDto>.Failure(error, 400);
        }

        var table = await _dbContext.GameTables
            .Include(t => t.ClubEvent)
            .FirstOrDefaultAsync(t => t.Id == request.GameTableId && !t.IsDeleted, cancellationToken);

        if (table is null || table.ClubEvent is null || !table.ClubEvent.IsActive || table.ClubEvent.IsDeleted)
        {
            return ServiceResult<ReservationResultDto>.Failure(
                "Seçilen oyun masası bulunamadı veya etkinlik kayıtlara kapandı.", 404);
        }

        if (table.CurrentPlayers >= table.MaxPlayers)
        {
            return ServiceResult<ReservationResultDto>.Failure(
                $"Masa #{table.TableNumber} ({table.GameTitle}) kontenjanı tamamen dolmuştur ({table.MaxPlayers}/{table.MaxPlayers}).", 409);
        }

        bool alreadyRegistered = await _dbContext.TableReservations
            .AnyAsync(r =>
                r.GameTableId == request.GameTableId &&
                r.ContactHandle.ToLower() == request.ContactHandle.Trim().ToLower() &&
                !r.IsDeleted,
                cancellationToken);

        if (alreadyRegistered)
        {
            return ServiceResult<ReservationResultDto>.Failure(
                "Bu iletişim bilgisi ile seçilen masada zaten aktif bir kaydınız bulunuyor.", 409);
        }

        var reservation = new TableReservation
        {
            Id = Guid.NewGuid(),
            GameTableId = table.Id,
            PlayerName = request.PlayerName.Trim(),
            ContactHandle = request.ContactHandle.Trim(),
            ExperienceLevel = request.ExperienceLevel,
            Notes = request.Notes?.Trim(),
            IsApproved = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        table.CurrentPlayers += 1;
        _dbContext.TableReservations.Add(reservation);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Masa #{TableNumber} için eşzamanlı koltuk seçimi çakışması yakalandı.", table.TableNumber);
            return ServiceResult<ReservationResultDto>.Failure(
                "Son koltuk saniyeler önce başka bir oyuncu tarafından alındı. Lütfen diğer masaları kontrol edin.", 409);
        }

        // Önbelleği temizle ve statik JSON dosyasını arka planda güncelle
        await _hybridCache.RemoveAsync(ActiveEventsCacheKey, cancellationToken);
        try
        {
            await _snapshotService.ExportEventsSnapshotAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rezervasyon sonrası statik events.snapshot.json güncellenirken hata oluştu.");
        }

        _logger.LogInformation("Yeni koltuk rezervasyonu kaydedildi. Masa #{TableNumber}, Katılımcı: {Player}",
            table.TableNumber, reservation.PlayerName);

        return ServiceResult<ReservationResultDto>.Success(new ReservationResultDto(
            ReservationId: reservation.Id,
            TableId: table.Id,
            TableNumber: table.TableNumber,
            GameTitle: table.GameTitle,
            CurrentPlayers: table.CurrentPlayers,
            MaxPlayers: table.MaxPlayers));
    }

    public async Task<ServiceResult<Guid>> CreateEventAsync(
        CreateEventDto request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _eventValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            string error = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return ServiceResult<Guid>.Failure(error, 400);
        }

        bool slugExists = await _dbContext.ClubEvents.AnyAsync(e => e.Slug == request.Slug, cancellationToken);
        if (slugExists)
            return ServiceResult<Guid>.Failure("Bu SEO kısa adına (slug) sahip bir etkinlik zaten mevcut.", 409);

        var newEvent = new ClubEvent
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Slug = request.Slug.Trim().ToLowerInvariant(),
            TldrSummary = request.TldrSummary.Trim(),
            StartDateUtc = request.StartDateUtc,
            EndDateUtc = request.EndDateUtc,
            VenueName = request.VenueName.Trim(),
            VenueAddress = request.VenueAddress.Trim(),
            MeetupEventUrl = request.MeetupEventUrl?.Trim(),
            ExternalRegistrationUrl = request.ExternalRegistrationUrl?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _dbContext.ClubEvents.Add(newEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync(ActiveEventsCacheKey, cancellationToken);
        await _snapshotService.ExportAllSnapshotsAsync(cancellationToken);

        _logger.LogInformation("Yeni Cumartesi etkinliği oluşturuldu: {Title} ({Id})", newEvent.Title, newEvent.Id);
        return ServiceResult<Guid>.Success(newEvent.Id, 201);
    }

    public async Task<ServiceResult<Guid>> AddTableAsync(
        CreateGameTableDto request,
        CancellationToken cancellationToken = default)
    {
        var clubEvent = await _dbContext.ClubEvents.FirstOrDefaultAsync(
            e => e.Id == request.ClubEventId && !e.IsDeleted,
            cancellationToken);

        if (clubEvent is null)
            return ServiceResult<Guid>.Failure("Bağlı etkinlik bulunamadı.", 404);

        bool tableNumberExists = await _dbContext.GameTables.AnyAsync(
            t => t.ClubEventId == request.ClubEventId && t.TableNumber == request.TableNumber && !t.IsDeleted,
            cancellationToken);

        if (tableNumberExists)
            return ServiceResult<Guid>.Failure($"Bu etkinlikte #{request.TableNumber} numaralı masa zaten tanımlı.", 409);

        var newTable = new GameTable
        {
            Id = Guid.NewGuid(),
            ClubEventId = request.ClubEventId,
            TableNumber = request.TableNumber,
            GameTitle = request.GameTitle.Trim(),
            Category = request.Category.Trim(),
            HostOrDmName = request.HostOrDmName.Trim(),
            MaxPlayers = request.MaxPlayers,
            CurrentPlayers = 0,
            IsBeginnerFriendly = request.IsBeginnerFriendly,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _dbContext.GameTables.Add(newTable);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync(ActiveEventsCacheKey, cancellationToken);
        await _snapshotService.ExportEventsSnapshotAsync(cancellationToken);

        _logger.LogInformation("Etkinliğe yeni masa eklendi: #{Number} - {Title}", newTable.TableNumber, newTable.GameTitle);
        return ServiceResult<Guid>.Success(newTable.Id, 201);
    }

    public async Task<ServiceResult<bool>> CancelReservationAsync(
        Guid reservationId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.TableReservations
            .Include(r => r.GameTable)
            .FirstOrDefaultAsync(r => r.Id == reservationId && !r.IsDeleted, cancellationToken);

        if (reservation is null)
            return ServiceResult<bool>.Failure("Rezervasyon bulunamadı.", 404);

        reservation.IsDeleted = true;
        if (reservation.GameTable is not null && reservation.GameTable.CurrentPlayers > 0)
        {
            reservation.GameTable.CurrentPlayers -= 1;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync(ActiveEventsCacheKey, cancellationToken);
        await _snapshotService.ExportEventsSnapshotAsync(cancellationToken);

        _logger.LogInformation("Rezervasyon iptal edildi: {ReservationId}", reservationId);
        return ServiceResult<bool>.Success(true);
    }

    private async Task<IReadOnlyList<EventDetailDto>> FetchActiveEventsFromDbAsync(CancellationToken ct)
    {
        return await _dbContext.ClubEvents
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsActive)
            .OrderBy(e => e.StartDateUtc)
            .Select(e => new EventDetailDto(
                e.Id,
                e.Title,
                e.Slug,
                e.TldrSummary,
                e.StartDateUtc,
                e.EndDateUtc,
                e.VenueName,
                e.VenueAddress,
                e.MeetupEventUrl,
                e.ExternalRegistrationUrl,
                e.Tables
                    .Where(t => !t.IsDeleted)
                    .OrderBy(t => t.TableNumber)
                    .Select(t => new GameTableDto(
                        t.Id,
                        t.TableNumber,
                        t.GameTitle,
                        t.Category,
                        t.HostOrDmName,
                        t.MaxPlayers,
                        t.CurrentPlayers,
                        t.IsBeginnerFriendly))
                    .ToList()))
            .ToListAsync(ct);
    }

    private static EventDetailDto MapToEventDetailDto(ClubEvent ev)
    {
        return new EventDetailDto(
            ev.Id,
            ev.Title,
            ev.Slug,
            ev.TldrSummary,
            ev.StartDateUtc,
            ev.EndDateUtc,
            ev.VenueName,
            ev.VenueAddress,
            ev.MeetupEventUrl,
            ev.ExternalRegistrationUrl,
            ev.Tables
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.TableNumber)
                .Select(t => new GameTableDto(
                    t.Id,
                    t.TableNumber,
                    t.GameTitle,
                    t.Category,
                    t.HostOrDmName,
                    t.MaxPlayers,
                    t.CurrentPlayers,
                    t.IsBeginnerFriendly))
                .ToList());
    }
}
