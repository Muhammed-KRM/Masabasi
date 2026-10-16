namespace Masabasi.Data.Entities;

/// <summary>
/// Bir Cumartesi etkinliği içinde açılan tekil oyun masasını (Kutu Oyunu veya FRP/D&D) temsil eder.
/// </summary>
public sealed class GameTable : BaseEntity
{
    /// <summary>Bağlı olduğu Cumartesi etkinliğinin kimliği.</summary>
    public Guid ClubEventId { get; set; }

    /// <summary>Fiziksel masa numarası (Örn: 12 numaralı masa).</summary>
    public int TableNumber { get; set; }

    /// <summary>Masada oynanacak oyunun veya FRP senaryosunun adı (Örn: "Secret Hitler / Kavgam" veya "D&D 5e: Curse of Strahd").</summary>
    public required string GameTitle { get; set; }

    /// <summary>Masa kategorisi: "BoardGame", "SocialDeduction", "FRP_DnD", "CardGame".</summary>
    public required string Category { get; set; }

    /// <summary>Masa yöneticisi (Dungeon Master veya Oyun Anlatıcısı).</summary>
    public required string HostOrDmName { get; set; }

    /// <summary>Masadaki maksimum oyuncu kapasitesi.</summary>
    public int MaxPlayers { get; set; }

    /// <summary>Masadaki mevcut onaylı oyuncu sayısı.</summary>
    public int CurrentPlayers { get; set; }

    /// <summary>Oyunu hiç bilmeyen yeni katılımcılar için uygun mu?</summary>
    public bool IsBeginnerFriendly { get; set; } = true;

    /// <summary>Eşzamanlı koltuk seçiminde çakışmayı (Overbooking) önleyen iyimser kilitleme (Optimistic Concurrency) sürümü.</summary>
    public uint RowVersion { get; set; }

    /// <summary>Masaya ev sahipliği yapan etkinlik referansı.</summary>
    public ClubEvent? ClubEvent { get; set; }

    /// <summary>Masaya kayıt olan oyuncular.</summary>
    public ICollection<TableReservation> Reservations { get; set; } = new List<TableReservation>();
}
