using System.Linq.Expressions;
using Masabasi.Data.Entities;

namespace Masabasi.Data.Repositories;

/// <summary>
/// Tüm veri tabanı varlıkları için temel CRUD ve sorgulama işlemlerini tanımlayan jenerik depo sözleşmesi.
/// </summary>
/// <typeparam name="T">BaseEntity sınıfından türeyen varlık tipi.</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>Birincil anahtara göre tekil kayıt getirir.</summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Tablodaki tüm aktif kayıtları getirir.</summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Belirli bir koşula uyan tüm kayıtları getirir.</summary>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Belirli bir koşula uyan ilk kaydı veya varsayılan değeri getirir.</summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Belirli bir koşula uyan herhangi bir kayıt olup olmadığını kontrol eder.</summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Tablodaki veya belirli bir koşula uyan kayıtların toplam sayısını döndürür.</summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>Yeni bir varlığı veritabanına ekler.</summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Birden fazla varlığı topluca veritabanına ekler.</summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>Mevcut bir varlığı güncelleme durumuna alır.</summary>
    void Update(T entity);

    /// <summary>Bir varlığı silinmek üzere işaretler (Soft Delete).</summary>
    void Delete(T entity);

    /// <summary>Özel LINQ sorguları için IQueryable arayüzünü sağlar.</summary>
    IQueryable<T> Query();

    /// <summary>Tüm değişiklikleri veritabanına kaydeder.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
