using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Masabasi.Data.Repositories;

/// <summary>
/// IEventRepository sözleşmesinin EF Core 10 implementasyonu.
/// </summary>
public sealed class EventRepository : GenericRepository<ClubEvent>, IEventRepository
{
    public EventRepository(MasabasiDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ClubEvent>> GetUpcomingEventsWithTablesAsync(
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.IsActive && !e.IsDeleted)
            .OrderBy(e => e.StartDateUtc)
            .Take(count)
            .Include(e => e.Tables.Where(t => !t.IsDeleted).OrderBy(t => t.TableNumber))
            .ToListAsync(cancellationToken);
    }

    public async Task<ClubEvent?> GetEventWithTablesBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(e => e.Tables.Where(t => !t.IsDeleted).OrderBy(t => t.TableNumber))
                .ThenInclude(t => t.Reservations.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(e => e.Slug == slug && !e.IsDeleted, cancellationToken);
    }

    public async Task<ClubEvent?> GetEventWithTablesByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Tables.Where(t => !t.IsDeleted).OrderBy(t => t.TableNumber))
                .ThenInclude(t => t.Reservations.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ClubEvent>> GetAllEventsWithTablesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.StartDateUtc)
            .Include(e => e.Tables.Where(t => !t.IsDeleted).OrderBy(t => t.TableNumber))
            .ToListAsync(cancellationToken);
    }
}
