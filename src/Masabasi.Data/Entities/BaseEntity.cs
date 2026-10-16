namespace Masabasi.Data.Entities;

/// <summary>
/// Masabaşı Oyun Kulübü veri tabanı varlıkları için temel kimlik ve denetim (audit) alanlarını tanımlayan soyut sınıf.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Varlığın benzersiz birincil anahtarı (GUID).</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Kaydın UTC cinsinden oluşturulma zaman damgası.</summary>
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Kaydın UTC cinsinden son güncellenme zaman damgası.</summary>
    public DateTimeOffset? UpdatedAtUtc { get; set; }

    /// <summary>Yumuşak silme (Soft Delete) bayrağı.</summary>
    public bool IsDeleted { get; set; } = false;
}
