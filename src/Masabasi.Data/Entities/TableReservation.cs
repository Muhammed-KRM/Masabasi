namespace Masabasi.Data.Entities;

/// <summary>
/// Bir oyuncunun belirli bir oyun masasına yaptığı koltuk rezervasyonunu temsil eden varlık.
/// </summary>
public sealed class TableReservation : BaseEntity
{
    /// <summary>Rezervasyon yapılan oyun masasının kimliği.</summary>
    public Guid GameTableId { get; set; }

    /// <summary>Oyuncunun adı ve soyadı / takma adı.</summary>
    public required string PlayerName { get; set; }

    /// <summary>İletişim bilgisi (Telefon numarası veya @instagram kullanıcı adı).</summary>
    public required string ContactHandle { get; set; }

    /// <summary>Oyuncunun deneyim seviyesi: "Beginner" (Yeni), "Intermediate" (Orta), "Veteran" (Deneyimli).</summary>
    public string ExperienceLevel { get; set; } = "Beginner";

    /// <summary>Oyuncunun masa yöneticisine iletmek istediği not (Örn: "D&D 5e Wizard karakterim hazır" veya "İlk defa oynayacağım").</summary>
    public string? Notes { get; set; }

    /// <summary>Rezervasyonun masa yöneticisi veya admin tarafından onaylanma durumu.</summary>
    public bool IsApproved { get; set; } = true;

    /// <summary>Rezervasyon yapılan masa referansı.</summary>
    public GameTable? GameTable { get; set; }
}
