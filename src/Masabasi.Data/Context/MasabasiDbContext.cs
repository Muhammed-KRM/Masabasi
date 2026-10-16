using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Masabasi.Data.Context;

/// <summary>
/// Masabaşı Oyun Kulübü ana Entity Framework Core veritabanı bağlamı.
/// Audit zaman damgalarını otomatik yönetir ve varlık konfigürasyonlarını yükler.
/// </summary>
public class MasabasiDbContext : DbContext
{
    public MasabasiDbContext(DbContextOptions<MasabasiDbContext> options)
        : base(options)
    {
    }

    /// <summary>Cumartesi etkinlikleri tablosu.</summary>
    public DbSet<ClubEvent> ClubEvents => Set<ClubEvent>();

    /// <summary>Etkinliklerde açılan kutu oyunu ve FRP masaları tablosu.</summary>
    public DbSet<GameTable> GameTables => Set<GameTable>();

    /// <summary>Masalara yapılan oyuncu koltuk rezervasyonları tablosu.</summary>
    public DbSet<TableReservation> TableReservations => Set<TableReservation>();

    /// <summary>Kulüp kutu oyunu ve kural rehberleri kütüphanesi tablosu.</summary>
    public DbSet<BoardGame> BoardGames => Set<BoardGame>();

    /// <summary>SEO Topic Cluster blog yazıları ve kural rehberleri tablosu.</summary>
    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bu derlemedeki tüm Fluent API IEntityTypeConfiguration sınıflarını otomatik uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasabasiDbContext).Assembly);

        // Global Soft Delete filtreleri
        modelBuilder.Entity<ClubEvent>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<GameTable>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<TableReservation>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<BoardGame>().HasQueryFilter(g => !g.IsDeleted);
        modelBuilder.Entity<Article>().HasQueryFilter(a => !a.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAtUtc == default)
                {
                    entry.Entity.CreatedAtUtc = now;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }
    }
}
