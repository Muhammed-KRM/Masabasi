using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masabasi.Data.Seed;

/// <summary>
/// Masabaşı Oyun Kulübü için başlangıç verilerini (etkinlikler, masalar, kutu oyunları kataloğu ve SEO blog rehberleri) veritabanına ekleyen tohumlayıcı (seeder).
/// </summary>
public static class MasabasiDbSeeder
{
    public static async Task SeedAsync(MasabasiDbContext context, ILogger? logger = null)
    {
        // 1. Kutu Oyunları Kataloğu Kontrolü ve Eklenmesi
        if (!await context.BoardGames.AnyAsync())
        {
            logger?.LogInformation("Masabaşı başlangıç oyun kütüphanesi verileri ekleniyor...");

            var games = new List<BoardGame>
            {
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                    Title = "Secret Hitler (Kavgam)",
                    Slug = "secret-hitler-kurallari-ve-oynanis-rehberi",
                    Category = "SocialDeduction",
                    MinPlayers = 5,
                    MaxPlayers = 10,
                    RecommendedPlayerCount = "7-8 Oyuncu",
                    PlayTimeMinutes = 45,
                    ComplexityRating = 2.1,
                    TldrSummary = "Secret Hitler, 1930'lar Almanyasında liberaller ve gizli faşistler arasındaki siyasi mücadeleyi konu alan tempolu bir sosyal çıkarım ve blöf oyunudur.",
                    RulesMarkdown = "### Secret Hitler Nedir?\nSecret Hitler, 5-10 kişiyle oynanan ve oyuncuların birbirlerinin rollerini çözmeye çalıştığı bir sosyal çıkarım oyunudur...",
                    IsInClubInventory = true,
                    YearPublished = 2016
                },
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                    Title = "Catan (Settlers of Catan)",
                    Slug = "catan-nasil-oynanir-takas-ve-strateji-rehberi",
                    Category = "BoardGame",
                    MinPlayers = 3,
                    MaxPlayers = 4,
                    RecommendedPlayerCount = "4 Oyuncu",
                    PlayTimeMinutes = 75,
                    ComplexityRating = 2.3,
                    TldrSummary = "Catan, oyuncuların ada üzerindeki hammaddeleri (odun, tuğla, buğday, koyun, taş) toplayıp takas ederek yollar ve köyler kurduğu modern kutu oyunlarının öncüsüdür.",
                    RulesMarkdown = "### Catan Temel Kuralları\nCatan adasına hoş geldiniz! Oyunda amaç ilk 10 zafer puanına ulaşmaktır...",
                    IsInClubInventory = true,
                    YearPublished = 1995
                },
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                    Title = "Dungeons & Dragons 5th Edition (D&D 5e)",
                    Slug = "dnd-5e-sistemi-ve-masaustu-rol-yapma-rehberi",
                    Category = "FRP_DnD",
                    MinPlayers = 3,
                    MaxPlayers = 6,
                    RecommendedPlayerCount = "4-5 Oyuncu + 1 DM",
                    PlayTimeMinutes = 240,
                    ComplexityRating = 3.0,
                    TldrSummary = "Dungeons & Dragons 5e, hayal gücü, 20 yüzlü zarlar (d20) ve kural kitapları eşliğinde ortak hikaye anlatımı yapılan dünyanın en popüler masaüstü FRP sistemidir.",
                    RulesMarkdown = "### D&D 5e Temel Dinamikleri\nD&D'de bir Dungeon Master (DM) dünyayı anlatır, oyuncular ise kendi yarattıkları karakterleri yönetir...",
                    IsInClubInventory = true,
                    YearPublished = 2014
                },
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                    Title = "Blood on the Clocktower",
                    Slug = "blood-on-the-clocktower-social-deduction",
                    Category = "SocialDeduction",
                    MinPlayers = 7,
                    MaxPlayers = 20,
                    RecommendedPlayerCount = "10-12 Oyuncu",
                    PlayTimeMinutes = 90,
                    ComplexityRating = 3.2,
                    TldrSummary = "Kasabada bir iblis dolaşıyor! Gece cinayetler işlenirken, ölen oyuncuların bile oyundan elenmeden konuşmaya ve oy vermeye devam ettiği devasa sosyal çıkarım başyapıtı.",
                    RulesMarkdown = "### Blood on the Clocktower Nasıl Oynanır?\nOyunda hikaye anlatıcısı (Storyteller) oyunu yönetir. Oyuncular köylü veya minyon/şeytan rollerini gizlice alır...",
                    IsInClubInventory = true,
                    YearPublished = 2022
                },
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111105"),
                    Title = "The Resistance: Avalon",
                    Slug = "avalon-kral-arthur-ve-mordred-sosyal-cikarim",
                    Category = "SocialDeduction",
                    MinPlayers = 5,
                    MaxPlayers = 10,
                    RecommendedPlayerCount = "7-8 Oyuncu",
                    PlayTimeMinutes = 40,
                    ComplexityRating = 2.4,
                    TldrSummary = "Kral Arthur'un sadık şövalyeleri ile Mordred'in hain hizmetkarlarının kutsal görevlerde çarpıştığı, Merlin ve Assassin rolleriyle derinleşen efsanevi parti oyunu.",
                    RulesMarkdown = "### Avalon Kuralları\nArthur'un şövalyeleri 3 başarılı göreve ulaşmaya çalışırken, hainler görevleri baltalamaya çalışır...",
                    IsInClubInventory = true,
                    YearPublished = 2012
                }
            };

            await context.BoardGames.AddRangeAsync(games);
        }

        // 2. SEO Topic Cluster Makale ve Rehberleri
        if (!await context.Articles.AnyAsync())
        {
            logger?.LogInformation("Masabaşı SEO Topic Cluster rehberleri ekleniyor...");

            var articles = new List<Article>
            {
                new()
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                    Title = "D&D 5e Nasıl Başlanır? İlk Kez FRP Oynayacaklar İçin Başlangıç Rehberi",
                    Slug = "dnd-5e-nasil-baslanir-ilk-kez-frp-oynayacaklar-icin-rehber",
                    TopicCluster = "FrpDnD",
                    TldrSummary = "D&D oynamak için kuralları ezberlemenize gerek yok. Masabaşı Oyun Kulübü'nde Cumartesi günleri açılan yeni başlayan dostu masalarla karakter kağıdınızı oluşturup maceraya adım atabilirsiniz.",
                    MetaDescription = "İstanbul'da D&D 5e oynamak isteyenler için adım adım başlangıç kılavuzu. Karakter oluşturma, d20 zar sistemi ve Cumartesi FRP masalarımız hakkında bilgi edinin.",
                    ContentMarkdown = @"## Dungeons & Dragons'a Giriş: Hayal Gücünün Masaya Taşınması

Masaüstü rol yapma oyunları (FRP), bilgisayar oyunlarının sunduğu sınırları ortadan kaldıran, tamamen oyuncuların kararlarına göre şekillenen bir deneyimdir.

### İlk Adım: Karakterinizi Tanımlamak
D&D 5e sisteminde bir karakter yaratırken iki temel seçim yaparsınız:
1. **Irk (Race):** Elf, Cüce, İnsan, Tiefling ve daha fazlası.
2. **Sınıf (Class):** Büyücü (Wizard), Savaşçı (Fighter), Düzenbaz (Rogue), Ruhban (Cleric).

### Masabaşı Oyun Kulübü'nde Cumartesi Masaları
Kulübümüzde her Cumartesi yeni başlayanlar için özel hazırlanmış **One-Shot** senaryoları açılmaktadır. Zarlarınızı ve karakter kağıtlarınızı biz sağlıyoruz!",
                    AuthorName = "Efe (Dungeon Master)",
                    EstimatedReadTimeMinutes = 6,
                    IsPublished = true,
                    PublishedAtUtc = DateTimeOffset.UtcNow.AddDays(-10),
                    FaqJson = @"[
                        {""question"": ""D&D oynamak için kendi zarlarıma ihtiyacım var mı?"", ""answer"": ""Hayır, kulübümüzde tüm zarlar, figürler ve haritalar hazır olarak temin edilmektedir.""},
                        {""question"": ""Daha önce hiç oynamadım, masada sorun yaşar mıyım?"", ""answer"": ""Kesinlikle hayır. 'Yeni Başlayan Dostu' etiketli masalarımızda DM kuralları adım adım anlatır.""}
                    ]"
                },
                new()
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                    Title = "Secret Hitler Taktikleri: Liberal ve Faşist Rollerinde Kazanma Sanatı",
                    Slug = "secret-hitler-taktikleri-kazanma-stratejileri",
                    TopicCluster = "KutuOyunlari",
                    TldrSummary = "Secret Hitler masasında masum bir liberal rolü mü oynuyorsunuz yoksa gizli bir faşist misiniz? İşte güven kazanma, kart sayma ve şansölye seçiminde ustalaşma yöntemleri.",
                    MetaDescription = "Secret Hitler oyununda kazanmak için profesyonel taktikler. Liberal politikalarını geçirme, Hitler'i koruma ve blöf tespit yöntemleri Masabaşı'nda.",
                    ContentMarkdown = @"## Sosyal Çıkarımın Zirvesi: Secret Hitler

Secret Hitler, güven ve ihanetin ince bir çizgide yürüdüğü eşsiz bir sosyal çıkarım oyunudur.

### Liberaller İçin Kritik Taktikler
- **Kart İstatistiği Tutun:** Destedeki 6 Liberal ve 11 Faşist politikasının dağılımını iyi takip edin.
- **Şansölye Güven Zinciri Kurun:** Temiz olduğundan emin olduğunuz oyuncularla hükümet kurmaya özen gösterin.",
                    AuthorName = "Masabaşı Strateji Ekibi",
                    EstimatedReadTimeMinutes = 5,
                    IsPublished = true,
                    PublishedAtUtc = DateTimeOffset.UtcNow.AddDays(-7)
                },
                new()
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222203"),
                    Title = "Baldur's Gate 3'ten Gerçek Masaya: Bilgisayardan D&D Masasına Geçiş Rehberi",
                    Slug = "baldurs-gate-3-ten-gercek-masaya-dnd-gecis-rehberi",
                    TopicCluster = "DijitaldenMasaustune",
                    TldrSummary = "Baldur's Gate 3'ü sevdiniz ve gerçek D&D oynamak mı istiyorsunuz? Bilgisayar oyunundaki mekaniklerin masaüstündeki karşılıkları ve geçişte bilmeniz gerekenler.",
                    MetaDescription = "Baldur's Gate 3 oyuncuları için gerçek masaüstü D&D 5e geçiş kılavuzu. Aksiyon ekonomisi, zar atma heyecanı ve canlı grup oyunu farkları.",
                    ContentMarkdown = @"## BG3 Sevdalıları İçin Masaüstü Dünyası

Larian Studios'un başyapıtı Baldur's Gate 3, aslında D&D 5e kural setinin dijital bir uyarlamasıdır.

### Ekranda Olmayanlar: Masadaki Özgürlük
Video oyununda oyun motorunun izin verdiği kadar seçeneğiniz vardır. Ancak masaüstünde DM ile birlikte aklınıza gelen her yaratıcı hamleyi deneyebilirsiniz!",
                    AuthorName = "Caner (DM & Gamer)",
                    EstimatedReadTimeMinutes = 6,
                    IsPublished = true,
                    PublishedAtUtc = DateTimeOffset.UtcNow.AddDays(-4)
                }
            };

            await context.Articles.AddRangeAsync(articles);
        }

        // 3. Yaklaşan Cumartesi Etkinliği ve Masalar
        if (!await context.ClubEvents.AnyAsync())
        {
            logger?.LogInformation("Masabaşı örnek Cumartesi etkinliği ve masalar ekleniyor...");

            // Önümüzdeki Cumartesi tarihini hesapla
            var today = DateTime.UtcNow;
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
            if (daysUntilSaturday == 0) daysUntilSaturday = 7;
            var saturdayDate = today.AddDays(daysUntilSaturday).Date;
            var eventStartUtc = new DateTimeOffset(saturdayDate.Year, saturdayDate.Month, saturdayDate.Day, 11, 0, 0, TimeSpan.Zero);
            var eventEndUtc = eventStartUtc.AddHours(7);

            var saturdayEvent = new ClubEvent
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                Title = $"Masabaşı Cumartesi Buluşması: Kutu Oyunları & FRP Masaları ({saturdayDate:dd MMMM yyyy})",
                Slug = $"cumartesi-bulusmasi-{saturdayDate:dd-MMMM-yyyy}".ToLower().Replace(" ", "-"),
                TldrSummary = "Masabaşı Oyun Kulübü olarak bu Cumartesi Kadıköy'de modern kutu oyunları (Catan, Ticket to Ride, Avalon) ve yeni başlayanlara açık D&D 5e FRP masaları için buluşuyoruz. Kurallar masada anlatılacaktır.",
                StartDateUtc = eventStartUtc,
                EndDateUtc = eventEndUtc,
                VenueName = "Masabaşı Buluşma Mekanı - Kadıköy",
                VenueAddress = "Kadıköy / İstanbul (Detaylı adres kayıt onayında iletilir)",
                MeetupEventUrl = "https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/events/316644892/",
                ExternalRegistrationUrl = "https://www.instagram.com/masabasikulubu/",
                IsActive = true,
                Tables = new List<GameTable>
                {
                    new()
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444401"),
                        TableNumber = 1,
                        GameTitle = "Secret Hitler (Kavgam) & Avalon — Sosyal Çıkarım Masası",
                        Category = "SocialDeduction",
                        HostOrDmName = "Masabaşı Moderatör Ekibi",
                        MaxPlayers = 10,
                        CurrentPlayers = 7,
                        IsBeginnerFriendly = true,
                        Reservations = new List<TableReservation>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                PlayerName = "Ahmet Y.",
                                ContactHandle = "@ahmety_boardgame",
                                ExperienceLevel = "Beginner",
                                Notes = "İlk kez Secret Hitler oynayacağım."
                            },
                            new()
                            {
                                Id = Guid.NewGuid(),
                                PlayerName = "Zeynep K.",
                                ContactHandle = "@zeynepk",
                                ExperienceLevel = "Intermediate"
                            }
                        }
                    },
                    new()
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444402"),
                        TableNumber = 2,
                        GameTitle = "Dungeons & Dragons 5e: Zindanların Çağrısı (One-Shot Seviye 1-3)",
                        Category = "FRP_DnD",
                        HostOrDmName = "DM Kerem",
                        MaxPlayers = 5,
                        CurrentPlayers = 4,
                        IsBeginnerFriendly = true,
                        Reservations = new List<TableReservation>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                PlayerName = "Burak T.",
                                ContactHandle = "05321112233",
                                ExperienceLevel = "Beginner",
                                Notes = "Level 2 Rogue karakterim var."
                            }
                        }
                    },
                    new()
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444403"),
                        TableNumber = 3,
                        GameTitle = "Catan & Ticket to Ride: Europe — Strateji Masası",
                        Category = "BoardGame",
                        HostOrDmName = "Oyun Anlatıcısı Selin",
                        MaxPlayers = 5,
                        CurrentPlayers = 3,
                        IsBeginnerFriendly = true
                    },
                    new()
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444404"),
                        TableNumber = 4,
                        GameTitle = "Blood on the Clocktower — Büyük Kasaba Gecesi",
                        Category = "SocialDeduction",
                        HostOrDmName = "Storyteller Efe",
                        MaxPlayers = 12,
                        CurrentPlayers = 11,
                        IsBeginnerFriendly = true
                    }
                }
            };

            await context.ClubEvents.AddAsync(saturdayEvent);
        }

        await context.SaveChangesAsync();
        logger?.LogInformation("Masabaşı veritabanı tohumlama işlemi tamamlandı.");
    }
}
