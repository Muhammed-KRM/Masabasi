namespace Masabasi.Data.Entities;

/// <summary>
/// Kulübün her Cumartesi düzenlediği kutu oyunu ve FRP etkinliklerini temsil eden ana varlık.
/// </summary>
public sealed class ClubEvent : BaseEntity
{
    /// <summary>Etkinlik başlığı (Örn: "Masabaşı Cumartesi Buluşması #42 - Kutu Oyunu & D&D Gecesi").</summary>
    public required string Title { get; set; }

    /// <summary>SEO uyumlu URL kısa adı (Örn: "cumartesi-bulusmasi-42-17-ekim-2026").</summary>
    public required string Slug { get; set; }

    /// <summary>Yapay zeka (GEO) ve hızlı okuyucular için 40-60 kelimelik net etkinlik özeti.</summary>
    public required string TldrSummary { get; set; }

    /// <summary>Etkinlik başlangıç tarihi ve saati (UTC).</summary>
    public DateTimeOffset StartDateUtc { get; set; }

    /// <summary>Etkinlik bitiş tarihi ve saati (UTC).</summary>
    public DateTimeOffset EndDateUtc { get; set; }

    /// <summary>Buluşma mekanı adı (Örn: "Kadıköy Goblin / Periwinkle Kafe").</summary>
    public required string VenueName { get; set; }

    /// <summary>Mekanın açık adresi (Google Event Schema için zorunludur).</summary>
    public required string VenueAddress { get; set; }

    /// <summary>API kapalıyken veya yedek kanal olarak kullanılacak Meetup etkinlik linki.</summary>
    public string? MeetupEventUrl { get; set; }

    /// <summary>WhatsApp veya Instagram hızlı kayıt yönlendirme linki.</summary>
    public string? ExternalRegistrationUrl { get; set; }

    /// <summary>Etkinlik aktif ve kayıtlara açık mı?</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Bu etkinlikte açılan kutu oyunu ve FRP masaları.</summary>
    public ICollection<GameTable> Tables { get; set; } = new List<GameTable>();
}
