# Masabaşı Oyun Kulübü — Kapsamlı Teknik Mimari, SEO/GEO ve Uçtan Uca Geliştirme Spesifikasyonu

**Belge Sürümü:** 2.0 (Ekim 2026 Güncel Standartları)  
**Hedef Teknoloji Yığını:** .NET 10.0 (Clean N-Layer Architecture) + Angular 20/21 (`@angular/ssr` Hybrid Rendering & Signals) + PostgreSQL 17 + Statik JSON Snapshot (API-Optional / Offline-First Fallback)  
**Referans Kodlama ve Mimari Standardı:** [Muhammed-KRM/OtoEdit](https://www.google.com/search?q=https%3A%2F%2Fgithub.com%2FMuhammed-KRM%2FOtoEdit)  
**Resmi Topluluk Kanalları:** [Masabaşı Oyun Kulübü Instagram (@masabasikulubu)](https://www.google.com/search?q=https%3A%2F%2Fwww-fallback.instagram.com%2Fmasabasikulubu%2F) | [Meetup Etkinlik Sayfası](https://www.google.com/search?q=https%3A%2F%2Fwww.meetup.com%2Ftr-tr%2Fmasaba%25c5%259f%25c4%25b1-oyun-kulubu%2Fevents%2F316644892%2F%3FrecId%3D5cbbf29b-a43d-4fa1-9e67-38ab9c686cc4%26recSource%3Devent-search%26searchId%3Dab85b3b9-02a0-4ad5-92ae-8de7a7ff6d24%26eventOrigin%3Dfind_page%2524all) | [Facebook Topluluk Grubu](https://www.google.com/search?q=https%3A%2F%2Fwww.facebook.com%2Fgroups%2F14080585061%2Fuser%2F61595360127902%2F)


## BÖLÜM 1: PROJENİN AMACI VE STRATEJİK HEDEFLERİ

### 1.1. Vizyon ve Temel Problem Tanımı
**Masabaşı Oyun Kulübü**, İstanbul'da her Cumartesi günü düzenli olarak bir araya gelen; modern kutu oyunları (*Catan*, *Ticket to Ride*, *Carcassonne*), sosyal çıkarım/parti oyunları (*Secret Hitler / Kavgam*, *Avalon*, *Blood on the Clocktower*) ve masaüstü rol yapma oyunları (**FRP / Dungeons & Dragons 5e**, *Call of Cthulhu*, *Pathfinder*) oynayan fiziksel ve sosyal bir oyun topluluğudur.

Bu projenin temel amacı; kulübün sosyal medya ([Instagram](https://www.google.com/search?q=https%3A%2F%2Fwww-fallback.instagram.com%2Fmasabasikulubu%2F), [Meetup](https://www.google.com/search?q=https%3A%2F%2Fwww.meetup.com%2Ftr-tr%2Fmasaba%25c5%259f%25c4%25b1-oyun-kulubu%2Fevents%2F316644892%2F%3FrecId%3D5cbbf29b-a43d-4fa1-9e67-38ab9c686cc4%26recSource%3Devent-search%26searchId%3Dab85b3b9-02a0-4ad5-92ae-8de7a7ff6d24%26eventOrigin%3Dfind_page%2524all), [Facebook](https://www.google.com/search?q=https%3A%2F%2Fwww.facebook.com%2Fgroups%2F14080585061%2Fuser%2F61595360127902%2F)) ve WhatsApp kanallarına dağılmış olan dijital varlığını **tek bir merkezi, yüksek hızlı ve arama motoru odaklı web platformunda** toplamaktır.

### 1.2. Dört Temel Mühendislik ve İş Hedefi

1. **Google (SEO) ve Yapay Zeka Arama Motorlarında (GEO / AEO) Mutlak Görünürlük:**
   * İstanbul'da ve Türkiye genelinde *"kutu oyunu kulübü"*, *"İstanbul FRP etkinlikleri"*, *"Cumartesi D&D masası"*, *"Secret Hitler nasıl oynanır"* gibi hem yerel (Local SEO) hem de bilgi odaklı (Informational SEO) aramalarda ilk sırada yer almak.
   * Ekim 2026 itibarıyla arama trafiğinin büyük kısmını yönlendiren yapay zeka arama motorlarının (**Google AI Overviews / AI Mode**, **ChatGPT Search**, **Perplexity**, **Claude**) siteyi eksiksiz taramasını, anlamlandırmasını ve cevaplarında **Masabaşı Oyun Kulübü**'nü doğrudan kaynak ([llms.txt v2 standardı](https://www.google.com/search?q=https%3A%2F%2Fllmstxt.org%2Fchanges.html) ve [Schema.org](https://www.google.com/search?q=https%3A%2F%2Fdevelopers.google.com%2Fsearch%2Fdocs%2Fappearance%2Fstructured-data%2Fevent)) olarak göstermesini sağlamak.
2. **Hem API'li Hem API'siz (Dual-Mode / Offline-Resilient) Kesintisiz Çalışma:**
   * Sistem, **.NET 10 Web API** açıkken tam dinamik bir platform gibi çalışır: Admin panelinden etkinlik/masa açılır, oyuncular masalardaki boş koltuklara kayıt olur ve **SignalR** üzerinden kalan kontenjanlar (*"Masa 2: D&D 5e — 4/5 Kişi"*) anlık güncellenir.
   * Ancak **.NET API hiç host edilmediğinde, sunucu bakımda olduğunda, ücretsiz bulut katmanı uyku moduna (cold start) girdiğinde veya veritabanı bağlantısı koptuğunda** site asla çökmez. Ön yüz (Angular), otomatik olarak proje içindeki **Statik JSON Snapshot (`/public/data/*.json`)** verilerine geçiş yapar. Canlı kayıt formları ise kendiliğinden **Meetup / WhatsApp / Instagram DM** yönlendirme butonlarına dönüşür.
3. **Vitrin (Public) Trafiği ile Yönetim (Admin) Panelinin Tam İzolasyonu:**
   * Kullanıcı vitrinine (Ana sayfa, Etkinlikler, Oyun Rehberleri, Blog) aynı anda on binlerce ziyaretçi veya arama motoru botu girdiğinde, bu trafik **CDN / Statik Prerender (SSG) ve Önbellek (Cache)** katmanından karşılanır.
   * Admin paneli ve yönetim API uçları; ayrı bir paket (bundle), ayrı bir hız sınırlama havuzu (Rate Limiting Partition) ve isteğe bağlı ayrı bir alt alan adı (`admin.masabasikulubu.com`) üzerinden çalışarak vitrin trafiğinden **%100 izole** edilir.
4. **Cumartesi Etkinlik ve Masa/Koltuk Operasyonunun Otomasyonu:**
   * Her Cumartesi yapılan etkinliklerde hangi masada hangi kutu oyununun veya FRP senaryosunun (One-Shot / Campaign) oynanacağını, masa yöneticisini (DM / Oyun Anlatıcısı), zorluk seviyesini (*"Yeni Başlayan Dostu"*) ve masa kapasitesini şeffaf biçimde sergilemek.


## BÖLÜM 2: PROJEDE KESİN OLMASI GEREKENLER VE GELİŞTİRİRKEN UYULMASI GEREKEN ÖNEMLİ KURALLAR

Bu bölümdeki kurallar hem yazılım mimarisi (kod kalitesi) hem de SEO/Tasarım standartları açısından **tavizsiz (non-negotiable)** kabul edilmelidir.

### 2.1. Yazılım Mimarisi ve Kodlama Kuralları (`Muhammed-KRM/OtoEdit` Standardı)

Projenin backend ve frontend kod tabanı, [Muhammed-KRM/OtoEdit](https://www.google.com/search?q=https%3A%2F%2Fgithub.com%2FMuhammed-KRM%2FOtoEdit) reposundaki **Modüler N-Katmanlı Mimari (Clean N-Layer Architecture)** ve **SOLID** prensiplerini birebir takip eder:

1. **Katman Sorumlulukları ve Bağımlılık Yönü (`API -> Business -> Data`):**
   * **`Masabasi.API` (Sunum ve Geçit Katmanı):** Sadece HTTP/REST uçlarını (Controller veya Minimal Endpoint), SignalR Hub'larını (`TableHub`), Rate Limiting politikalarını, Global Exception Middleware'ini ve DI (Dependency Injection) kayıtlarını barındırır. **Controller içinde kesinlikle iş mantığı (`if/else` iş kuralları) veya doğrudan `DbContext` çağrısı yazılamaz.**
   * **`Masabasi.Business` (İş Mantığı Katmanı):** Tüm iş kuralları `Services/` altındaki `*Manager` sınıflarında (`EventManager`, `TableManager`, `ArticleManager`, `GameCatalogManager`, `StaticSnapshotManager`) yer alır. Her Manager sınıfı mutlaka `Interfaces/` altındaki bir arayüzü (`IEventService`, `ITableService`) implemente eder.
   * **`Masabasi.Data` (Veri Erişim Katmanı):** Entity Framework Core 10 (`MasabasiDbContext`), PostgreSQL bağlantısı, `Configurations/` altındaki `IEntityTypeConfiguration<T>` sınıfları (Data Annotations yerine **sadece Fluent API** kullanılır) ve `Repositories/` altındaki `GenericRepository<T>` sınıflarını içerir.
2. **SOLID ve Temiz Kod İlkeleri:**
   * **Single Responsibility (Tek Sorumluluk):** Bir sınıf yalnızca tek bir işten sorumludur. Örneğin `EventManager` sadece etkinlik iş kurallarını yönetir; statik JSON dosyası üretme işi `StaticSnapshotManager` sınıfına aittir.
   * **Dependency Inversion (Bağımlılıkların Tersine Çevrilmesi):** Üst katmanlar somut sınıflara (`EventRepository`) değil, soyutlamalara (`IEventRepository`) bağımlıdır. Tüm bağımlılıklar **Constructor Injection** ile alınır.
   * **Modüler DI Kayıt Uzantıları (Extension Methods):** Her katman kendi bağımlılıklarını `AddBusinessLayer()` ve `AddDataLayer()` uzantı metotlarıyla kaydeder; `Program.cs` şişirilmez.
3. **Standart İsimlendirme ve Dokümantasyon Kuralları:**
   * **C# (.NET 10):** Sınıf, arayüz, metot ve public property isimleri `PascalCase` (`CreateSaturdayEventAsync`); `private readonly` alanlar alt çizgi ile başlayan `_camelCase` (`_eventRepository`, `_snapshotService`, `_logger`) formatında yazılır.
   * **Türkçe XML Dokümantasyon Zorunluluğu:** Tüm `public` sınıf, arayüz, DTO ve metotların üzerinde ne işe yaradığını açıklayan Türkçe `/// <summary>` XML yorumları bulunmak zorundadır.
   * **Asenkron Programlama ve `CancellationToken`:** Veritabanı, dosya sistemi veya ağ erişimi yapan tüm metotlar `Async` soneki alır ve son parametre olarak `CancellationToken cancellationToken = default` kabul eder.
   * **Result Pattern (İstisna Yönetimi):** İş kuralları ihlallerinde (ör. *"Masa kontenjanı dolu"* veya *"Aynı telefon numarasıyla zaten kayıt var"*) `throw new Exception()` kullanılmaz; `ServiceResult<T>.Failure("Masa kontenjanı dolmuştur.", ErrorType.Conflict)` döndürülür.
4. **TypeScript / Angular 20+ Kuralları:**
   * Tüm bileşenler **Standalone Component** (`standalone: true`) olarak yazılır, `NgModule` kullanılmaz.
   * Durum yönetimi (State Management) için **Angular Signals** (`signal`, `computed`, `effect`) ve `inject()` fonksiyonu kullanılır.
   * Tüm bileşenlerde `changeDetection: ChangeDetectionStrategy.OnPush` zorunludur.
   * Dosya isimlendirmeleri `kebab-case` (`table-seat-card.component.ts`, `resilient-data.service.ts`) standardına uyar.

### 2.2. Hibrit Çalışma (API'li ve API'siz Mod) ve İzolasyon Kuralları

1. **Sıfır Kesinti / API-Optional Kuralı (`Resilient Fallback Pattern`):**
   * Sitenin halka açık hiçbir sayfası `.NET API`'nin ayakta olmasına **mutlak bağımlı olamaz**.
   * Angular veri servisleri (`EventDataService`, `GameCatalogDataService`, `ArticleDataService`), `environment.useStaticOnly` bayrağı `true` ise hiç API isteği atmadan doğrudan `/data/*.snapshot.json` dosyalarını okur.
   * `useStaticOnly: false` (Hibrit Mod) ise önce `.NET API`'ye istek atar; ancak API `1500ms` içinde yanıt vermezse veya `5xx / Network Error` dönerse, RxJS `timeout(1500)` ve `catchError()` operatörleri devreye girerek veriyi anında `/data/*.snapshot.json` dosyasından yükler.
2. **Otomatik Statik Snapshot Senkronizasyonu:**
   * Admin paneli üzerinden `.NET API` kullanılarak bir etkinlik, masa veya blog yazısı eklendiğinde/güncellendiğinde, `StaticSnapshotManager` arka planda `public/data/events.snapshot.json`, `public/data/games.snapshot.json`, `public/data/articles.snapshot.json`, `public/sitemap.xml` ve `public/llms.txt` dosyalarını otomatik olarak yeniden üretir.
   * Böylece Admin işini bitirip API sunucusunu kapatsa bile, site en güncel verilerle statik olarak çalışmaya devam eder.
3. **Admin ve Vitrin (Public) İzolasyon Kuralı:**
   * **Kod Paketi (Bundle) Ayrımı:** Admin paneli rotaları (`/admin/**`) sadece `loadChildren` ile tembel yüklenir (Lazy-Loaded) ve `RenderMode.Client` (CSR) olarak işaretlenir. Normal bir ziyaretçi siteye girdiğinde Admin paneline ait tek bir satır JS kodu bile tarayıcısına inmez.
   * **Sunucu / Hosting Ayrımı (İsteğe Bağlı Çift Dağıtım):** Mimari, tek bir komutla hem tek domain altında (`masabasikulubu.com` ve `masabasikulubu.com/admin`) hem de iki ayrı sub-domain altında (`masabasikulubu.com` -> Tamamen Statik CDN, `admin.masabasikulubu.com` -> API Bağlantılı Yönetim Uygulaması) yayınlanacak şekilde yapılandırılmıştır.
   * **API Kaynak Koruma Kuralı:** `.NET API` üzerinde halka açık okuma istekleri (`GET /api/v1/public/*`) bellek içi önbellekten (`OutputCache` / `HybridCache`) yanıtlanır ve veritabanını meşgul etmez. Böylece vitrine 50.000 kişi aynı anda girse dahi Admin panelinin (`POST/PUT /api/v1/admin/*`) veritabanı ve CPU kaynakları %100 korunur.

### 2.3. SEO ve GEO (Yapay Zeka İndeksleme) Kuralları — Olması ve Olmaması Gerekenler

| Kural Kategorisi | KESİNLİKLE OLMASI GEREKENLER (DO's) | KESİNLİKLE OLMAMASI GEREKENLER (DON'Ts) |
| :--- | :--- | :--- |
| **Sayfa ve İçerik Mimarisi** | **[Topic Cluster (Hub \& Spoke) Mimarisi](https://www.google.com/search?q=https%3A%2F%2Fdevelopers.google.com%2Fsearch%2Fdocs%2Ffundamentals%2Fcreating-helpful-content):** Ana merkez Kutu Oyunu ve FRP/D&D olmalı; Kart Oyunları (MTG, Yu-Gi-Oh), Dijital RPG (Baldur's Gate 3 -> D&D geçişi), Anime ve Cosplay konuları **masaüstü oyun kesişimiyle** birbirine iç linklerle bağlanmalı. | **[Doorway Pages ve Scaled Content Abuse](https://www.google.com/search?q=https%3A%2F%2Fdevelopers.google.com%2Fsearch%2Fdocs%2Fessentials%2Fspam-policies):** Sırf kelime yakalamak için footer'a yüzlerce içi boş, birbirinin kopyası veya kulüple alakasız ("En iyi 10 aksiyon filmi", "Valorant ayarları") çöp sayfa doldurulmamalı. |
| **Render Stratejisi** | **[Angular SSR \& SSG Prerendering](https://www.google.com/search?q=https%3A%2F%2Fangular.dev%2Fguide%2Frouting%2Frendering-strategies):** Tüm halka açık sayfalar derleme anında (`RenderMode.Prerender`) veya sunucuda (`RenderMode.Server`) tam HTML olarak üretilmeli; `curl` ile bakıldığında tüm metin, başlıklar ve JSON-LD kaynak kodda görünmeli. | **Saf Client-Side Rendering (CSR):** Halka açık sayfalarda boş `<app-root></app-root>` döndüren klasik SPA yapısı kullanılmamalı (AI botları JS çalıştırmadığı için siteyi boş görür). |
| **Yapay Zeka İndeksleme (GEO)** | **[llms.txt v2 Standardı](https://www.google.com/search?q=https%3A%2F%2Fllmstxt.org%2Fchanges.html) & Answer-First Blokları:** Kök dizinde `/llms.txt` ve `/llms-ctx.txt` bulunmalı. Her sayfanın başında `<aside aria-label="Özet Bilgi">` içinde 40-60 kelimelik net özet (TL;DR) yer almalı ([GEO Rehberi](https://www.google.com/search?q=https%3A%2F%2Fdetartech.com%2Fen%2Fblog%2Fai-search-engines-geo-guide)). | **PDF veya Görsel İçi Metin:** Etkinlik saatleri, masa listeleri veya oyun kuralları sadece Instagram afiş görseli (`.jpg`) içine gömülüp metinsiz bırakılmamalı. |
| **Yapısal Veri (Schema.org)** | **Zengin JSON-LD Şemaları:** Cumartesi buluşmaları için [Event Schema](https://www.google.com/search?q=https%3A%2F%2Fdevelopers.google.com%2Fsearch%2Fdocs%2Fappearance%2Fstructured-data%2Fevent), kulüp için [Organization Schema](https://www.google.com/search?q=https%3A%2F%2Fdevelopers.google.com%2Fsearch%2Fdocs%2Fappearance%2Fstructured-data%2Forganization), rehberler için [Article Schema](https://www.google.com/search?q=https%3A%2F%2Fdevelopers.google.com%2Fsearch%2Fdocs%2Fappearance%2Fstructured-data%2Farticle) ve `FAQPage` otomatik üretilmeli. | **Sahte / Tutarsız Schema Verisi:** Sayfada görünmeyen tarih veya fiyatları JSON-LD içine gizlemek (Google yapısal veri manuel cezasına yol açar). |
| **Core Web Vitals & Görseller** | **`NgOptimizedImage` & Modern Formatlar:** Tüm fotoğraflar `.webp` veya `.avif` formatında, `width`/`height` değerleri tanımlı (CLS = 0) ve Hero görseli `priority` işaretli olmalı. | **Optimize Edilmemiş Dev Görseller:** Doğrudan telefondan çekilmiş 5 MB boyutlu `.png/.jpg` fotoğrafları sıkıştırmadan yüklemek. |

### 2.4. Görsel Tasarım ve 60-30-10 Renk Teorisi Kuralları

Masabaşı Oyun Kulübü'nün logosundaki **altın halka, klasik serif tipografi ve buluşma mekanındaki adaçayı yeşili / sıcak ahşap** atmosferi temel alınarak [60-30-10 Renk Kuralı](https://www.google.com/search?q=https%3A%2F%2Fhype4.academy%2Farticles%2Fdesign%2F60-30-10-rule-in-ui) ([NN/g Renk Rehberi](https://www.google.com/search?q=https%3A%2F%2Fwww.nngroup.com%2Farticles%2Fcolor-enhance-design%2F)) uygulanır:

* **%60 Dominant (Arka Plan ve Geniş Okuma Yüzeyleri):**
  * *Koyu Tema (Varsayılan - "Taverna & Zindan" Modu):* Derin Arduvaz Kömürü (`#121619`) ve Koyu Çam Yüzeyi (`#1A2126`). Ana metin rengi göz yormayan Sıcak Parşömen Kremi (`#F4EFE6`).
  * *Açık Tema (Rehber / Gündüz Okuma Modu - "Parşömen & Kafe" Modu):* Sıcak Parşömen (`#FAF6F0`) ve Yumuşak Keten (`#F0E9DF`). Ana metin rengi Koyu Mürekkep (`#1C2226`).
* **%30 Sekonder (Marka Kimliği, Kartlar, Navbar, Footer ve Rozet Alanları):**
  * *Masabaşı Zümrüt / Adaçayı Yeşili:* `#1E3A34` (yüzey) ve `#2A524A` (vurgulu kenarlık/hover).
  * *Ceviz Ahşap Sıcak Tonu:* `#2C221E` (masa kartları ve FRP bilgi panelleri).
* **%10 Vurgu / Accent (CTA Butonları, Zar İkonları, Kontenjan Uyarıları):**
  * *Logo Altını / Pirinç:* `#D4AF37` (Hover: `#E5C158` — "Masada Yerini Ayırt" ana aksiyon butonları, aktif sekme işaretleri).
  * *Kritik Zar (D20 Nat 20) Kızılı:* `#E05A47` ("Son 1 Koltuk!", "Yeni Başlayan Dostu" rozeti, kritik bildirimler).
* **Tipografi Kuralları:**
  * **Başlık Fontu (`--font-display`):** `Cinzel` veya `Fraunces` (Türkçe karakter destekli, FRP ve klasik kutu oyunu ruhunu yansıtan serif).
  * **Gövde ve UI Fontu (`--font-sans`):** `Plus Jakarta Sans` (Uzun oyun kuralları ve blog yazılarında maksimum okunabilirlik).


## BÖLÜM 3: SİSTEM MİMARİSİ, TEKNOLOJİLER VE İZOLASYON STRATEJİSİ

### 3.1. Mimari Diyagram ve Çift Modlu (API + Statik Fallback) Çalışma Akışı

Sistem, **Clean N-Layer .NET 10 Backend** ve **Angular 20+ Hybrid SSR/SSG Frontend** üzerine kuruludur. En kritik mühendislik özelliği olan **"Hem API'li Hem API'siz Çalışabilme"** ve **"Admin / Vitrin İzolasyonu"** şu şekilde işler:

```text
====================================================================================================
                        ZİYARETÇİLER & YAPAY ZEKA BOTLARI (Googlebot, GPTBot, Perplexity)
====================================================================================================
                                              │
                                              ▼
                     [ Cloudflare CDN / Nginx Reverse Proxy & Edge Cache ]
                                              │
         ┌────────────────────────────────────┴────────────────────────────────────┐
         │ (%99 Trafik: Halka Açık Vitrin)                                         │ (İzole Trafik: Yönetim)
         ▼                                                                         ▼
┌───────────────────────────────────────────────────┐             ┌─────────────────────────────────┐
│  MASABAŞI PUBLIC WEB (Angular SSG / SSR)          │             │  MASABAŞI ADMIN (Angular CSR)   │
│  - Ana Sayfa, Etkinlikler, Oyun Kütüphanesi, Blog │             │  - /admin veya admin.domain.com │
│  - Prerendered HTML + JSON-LD + /llms.txt         │             │  - Sadece JWT + Admin Role      │
└─────────────────────────┬─────────────────────────┘             └────────────────┬────────────────┘
                          │                                                        │
                          ▼                                                        │
         ┌────────────────────────────────┐                                        │
         │ ResilientDataOrchestrator (UI) │                                        │
         └────────┬──────────────┬────────┘                                        │
                  │              │                                                 │
    (1. Tercih:   │              │ (2. Fallback: API Kapalıysa /                   │ (Doğrudan Yönetim
     API Açık mı?)│              │  Timeout > 1500ms / Static Mod)                 │  API Çağrıları)
                  ▼              ▼                                                 ▼
      ┌───────────────┐  ┌─────────────────────────────┐             ┌──────────────────────────────┐
      │ Public API    │  │ Statik Snapshot Katmanı     │             │ Admin API Havuzu (.NET 10)   │
      │ Havuzu        │  │ /public/data/*.snapshot.json│◄────────────┤ - POST/PUT Etkinlik & Masa   │
      │ (OutputCache  │  │ - events.snapshot.json      │ (Kayıt Anında│ - Blog & Oyun Rehberi Yayınla│
      │  + SignalR)   │  │ - games.snapshot.json       │  Otomatik   │ - StaticSnapshotManager ile  │
      └───────┬───────┘  │ - articles.snapshot.json    │  JSON + XML │   JSON/llms.txt/Sitemap Üret │
              │          └─────────────────────────────┘  Günceller) └──────────────┬───────────────┘
              └──────────────────────────────────────┬──────────────────────────────┘
                                                     ▼
                                     ┌───────────────────────────────┐
                                     │ Masabasi.Business & Data      │
                                     │ - EF Core 10 + PostgreSQL 17  │
                                     │ - HybridCache (Memory/Redis)  │
                                     └───────────────────────────────┘
```

### 3.2. "Tek UI mı, İki Ayrı Proje mi?" — Hosting ve Trafik İzolasyonu Analizi

Kullanıcı kısmına (vitrin) çok fazla kişinin girmesinin Admin panelini etkilememesi ve hosting maliyetinin artmaması için uygulanacak mimari strateji:

1. **Hosting Maliyeti ve Dağıtım Gerçeği:**
   * Eğer ön yüzü iki ayrı Angular uygulaması veya iki ayrı build çıktısı (`dist/masabasi-web` ve `dist/masabasi-admin`) olarak ayırırsan, **Cloudflare Pages, Vercel, Netlify veya kendi Nginx sunucunda bunun ekstra maliyeti 0 TL'dir (Tamamen Ücretsizdir)**. Çünkü statik/SSR ön yüz barındırma servisleri proje başına ücret almaz; aynı GitHub reposundan hem `masabasikulubu.com` (Vitrin) hem de `admin.masabasikulubu.com` (Admin) ücretsiz yayınlanabilir.
2. **Neden "Tek Repo İçinde İzole Lazy-Loaded `/admin` + Ayrı API Rate-Limit Havuzu" En Pratik Çözümdür?**
   * İki ayrı Angular projesi yönetmek yerine, tek bir Angular projesinde `/admin` rotasını `RenderMode.Client` (CSR) ve `canMatch: [adminAuthGuard]` ile tamamen ayrı bir JS chunk'ı olarak ayırmak, `Shared` modelleri (`EventDto`, `GameTableDto`) iki kez yazmanı engeller.
   * **Trafik Darboğazını Önleyen Asıl Mekanizma:** Vitrin sayfaları (`/` , `/etkinlikler`, `/oyunlar/*`, `/blog/*`) **SSG (Prerender)** ve **CDN Cache** kullandığı için, vitrine gelen 100.000 ziyaretçi `.NET API`'ye veya veritabanına **hiç dokunmaz**. API'ye giden tek istek `GET /api/v1/public/events/active-tables` (canlı masa doluluk sayısı) isteğidir; o da `.NET 10 OutputCache` ile bellekte (RAM) tutulduğu için CPU ve PostgreSQL'i %0.1 bile yormaz.
   * Üstelik `.NET 10` içinde `Microsoft.AspNetCore.RateLimiting` kullanılarak **Public Endpoint'ler** ile **Admin Endpoint'leri** iki ayrı kuyruğa (`PartitionedRateLimiter`) ayrılır. Public kuyruğu tamamen dolsa bile Admin istekleri kendi özel havuzundan (`AdminBypassPolicy`) anında işlenir.

### 3.3. Kullanılacak Teknolojiler ve Kütüphaneler Listesi

#### A. Backend (.NET 10.0 — `Masabasi.sln`)
| Katman | Paket / Kütüphane | Sürüm | Kullanım Amacı |
| :--- | :--- | :--- | :--- |
| **`Masabasi.API`** | `Microsoft.AspNetCore.OpenApi` / `Scalar.AspNetCore` | `10.0.x` | Modern OpenAPI dokümantasyonu ve API test arayüzü |
| **`Masabasi.API`** | `Microsoft.AspNetCore.SignalR` | `10.0.x` | Cumartesi masalarındaki kalan koltuk sayısını canlı yayınlamak (`TableHub`) |
| **`Masabasi.API`** | `Microsoft.AspNetCore.Authentication.JwtBearer` | `10.0.x` | Admin paneli ve DM (Oyun Yöneticisi) yetkilendirmesi |
| **`Masabasi.API`** | `Microsoft.AspNetCore.OutputCaching` | `10.0.x` | Public okuma uçlarını RAM'de önbellekleme (Admin'i koruma) |
| **`Masabasi.Business`** | `FluentValidation.DependencyInjectionExtensions` | `11.11.x` | İstek DTO'larının kurallı doğrulaması |
| **`Masabasi.Business`** | `Microsoft.Extensions.Caching.Hybrid` | `10.0.x` | .NET 10 L1 (In-Memory) + L2 (Redis) birleşik önbellek yönetimi |
| **`Masabasi.Business`** | `Markdig` | `0.38.x` | Blog ve oyun rehberlerindeki Markdown içeriklerini güvenli HTML'e ve `llms.txt` özetine çevirme |
| **`Masabasi.Data`** | `Microsoft.EntityFrameworkCore` | `10.0.x` | ORM ve veritabanı yönetimi |
| **`Masabasi.Data`** | `Npgsql.EntityFrameworkCore.PostgreSQL` | `10.0.x` | PostgreSQL 17 veritabanı sağlayıcısı |
| **`Masabasi.UnitTests`** | `xUnit`, `FluentAssertions`, `Moq` | `2.9.x` | İş katmanı ve Snapshot üreticisi birim testleri |

#### B. Frontend (Angular 20/21 — `Masabasi.Frontend`)
| Kategori | Paket / Teknoloji | Kullanım Amacı |
| :--- | :--- | :--- |
| **Çekirdek & SSR** | `@angular/core`, `@angular/ssr`, `@angular/platform-server` | Hibrit Render (SSG Prerender + SSR + CSR Admin) ve Incremental Hydration |
| **Stil & Tasarım** | `tailwindcss` + Özel SCSS CSS Variables (`_variables.scss`) | 60-30-10 renk teorisi token'ları, Koyu/Açık tema ve responsive grid |
| **Gerçek Zamanlı** | `@microsoft/signalr` | API aktifken masa kontenjan değişimlerini anlık dinleme |
| **SEO & İkonlar** | `lucide-angular` (veya özel SVG Sprite) | D20 zar, piyon, kılıç, büyü kitabı, masa ve takvim ikonları |


## BÖLÜM 4: AYRINTILI PROJE DOSYA YAPISI VE DOSYA GÖREVLERİ

Aşağıdaki dosya yapısı, [Muhammed-KRM/OtoEdit](https://www.google.com/search?q=https%3A%2F%2Fgithub.com%2FMuhammed-KRM%2FOtoEdit) reposundaki çözüm (solution) organizasyonu temel alınarak Masabaşı Oyun Kulübü'nün **Hibrit (API + Statik Fallback)** ve **SEO/GEO** gereksinimlerine göre tasarlanmıştır:

```text
MasabasiOyunKulubu/
├── docs/
│   └── Architecture-And-SEO-Spec.md               # Bu teknik spesifikasyon dokümanı
├── nginx/
│   └── nginx.conf                                 # Public (Cache'li) ve Admin/API (İzole) yönlendirme ayarları
├── scripts/
│   ├── export-static-snapshot.ps1                 # Veritabanından statik JSON, sitemap.xml ve llms.txt üreten CLI betiği
│   ├── baslat.bat                                 # Yerel geliştirme ortamını (Docker + API + Angular) başlatan betik
│   └── durdur.bat                                 # Конteynerleri ve servisleri durduran betik
├── src/
│   ├── Masabasi.API/                              # [KATMAN 1] ASP.NET Core 10.0 Sunum ve Geçit Katmanı
│   │   ├── Controllers/
│   │   │   ├── Public/                            # Halka açık, OutputCache + PublicRateLimit korumalı salt-okunur uçlar
│   │   │   │   ├── EventsPublicController.cs      # Cumartesi etkinlikleri ve masa listesi (GET)
│   │   │   │   ├── GamesPublicController.cs       # Kutu oyunu ve FRP kütüphanesi (GET)
│   │   │   │   ├── ArticlesPublicController.cs    # Blog, kural rehberleri ve yan kültür yazıları (GET)
│   │   │   │   └── ReservationsPublicController.cs# Masaya oyuncu ön kaydı oluşturma (POST - Turnstile/RateLimit korumalı)
│   │   │   └── Admin/                             # İzole AdminRateLimit + [Authorize(Roles="Admin,DungeonMaster")] uçları
│   │   │       ├── EventsAdminController.cs       # Etkinlik oluşturma, düzenleme, masa/DM atama (POST/PUT/DELETE)
│   │   │       ├── GamesAdminController.cs        # Kutu oyunu kütüphanesi yönetimi
│   │   │       ├── ArticlesAdminController.cs     # SEO uyumlu blog/rehber yayınlama
│   │   │       └── SnapshotAdminController.cs     # Tek tıkla statik JSON + llms.txt + sitemap.xml paketini üretme ucu
│   │   ├── Hubs/
│   │   │   └── TableHub.cs                        # Masa kontenjanı değiştiğinde bağlı istemcilere canlı bildirim atan SignalR Hub
│   │   ├── Middlewares/
│   │   │   ├── GlobalExceptionMiddleware.cs       # Tüm hataları yakalayıp standart ProblemDetails / ServiceResult formatında dönen katman
│   │   │   └── TrafficIsolationMiddleware.cs      # Admin ve Public isteklerinin metrik ve öncelik ayrımını yapan ara katman
│   │   ├── Extensions/
│   │   │   └── RateLimitingExtensions.cs          # PublicReadPolicy, PublicReservationPolicy ve AdminBypassPolicy tanımları
│   │   ├── appsettings.json                       # JWT, PostgreSQL, Snapshot dizin yolu ve Kulüp sabit ayarları
│   │   └── Program.cs                             # Uygulama giriş noktası ve katmanların bağlanması
│   │
│   ├── Masabasi.Business/                         # [KATMAN 2] İş Mantığı, Doğrulama ve Statik Üretim Katmanı
│   │   ├── Common/
│   │   │   ├── ServiceResult.cs                   # Başarı/Hata durumu, hata kodu ve mesaj taşıyan jenerik sarmalayıcı
│   │   │   └── PagedResult.cs                     # Sayfalanmış liste yanıt modeli
│   │   ├── DTOs/
│   │   │   ├── Events/                            # EventDetailDto, GameTableDto, CreateReservationDto,CreateEventDto
│   │   │   ├── Games/                             # BoardGameDto, GameFilterDto
│   │   │   └── Articles/                          # ArticleSummaryDto, ArticleDetailDto, SeoMetadataDto
│   │   ├── Interfaces/
│   │   │   ├── IEventService.cs                   # Etkinlik ve masa yönetimi sözleşmesi
│   │   │   ├── IGameCatalogService.cs             # Oyun kütüphanesi sözleşmesi
│   │   │   ├── IArticleService.cs                 # Blog ve Topic Cluster rehberleri sözleşmesi
│   │   │   └── IStaticSnapshotService.cs          # API'siz çalışma için JSON, sitemap.xml ve llms.txt üretici sözleşmesi
│   │   ├── Services/
│   │   │   ├── EventManager.cs                    # Cumartesi etkinlik ve masa kontenjan kuralları (Overbooking koruması)
│   │   │   ├── GameCatalogManager.cs              # Oyun filtreleme ve rehber eşleştirme mantığı
│   │   │   ├── ArticleManager.cs                  # Markdown -> HTML dönüşümü, okuma süresi ve otomatik TL;DR doğrulaması
│   │   │   └── StaticSnapshotManager.cs           # Veritabanı verisini /public/data/*.snapshot.json ve llms.txt olarak dışa aktaran motor
│   │   ├── Validators/
│   │   │   ├── CreateReservationValidator.cs      # İsim, iletişim ve masa kapasitesi FluentValidation kuralları
│   │   │   └── CreateArticleValidator.cs          # SEO başlık uzunluğu (50-60 kr), meta açıklama ve TL;DR zorunluluğu kontrolü
│   │   └── BusinessServiceRegistration.cs         # AddBusinessLayer() DI kayıt uzantısı
│   │
│   ├── Masabasi.Data/                             # [KATMAN 3] EF Core 10 Veri Erişim Katmanı
│   │   ├── Context/
│   │   │   └── MasabasiDbContext.cs               # PostgreSQL DbContext ve Audit (CreatedAt/UpdatedAt) interceptor'ı
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs                      # Id (Guid), CreatedAtUtc, UpdatedAtUtc, IsDeleted
│   │   │   ├── ClubEvent.cs                       # Cumartesi etkinliği (Başlık, Slug, Tarih, Mekan, MeetupUrl, Durum)
│   │   │   ├── GameTable.cs                       # Etkinlik içindeki masa (Masa No, Oyun/FRP Adı, DM/Anlatıcı, Min/Max Oyuncu)
│   │   │   ├── TableReservation.cs                # Masaya kayıt olan oyuncu bilgisi ve onay durumu
│   │   │   ├── BoardGame.cs                       # Kulüp envanterindeki kutu oyunu / FRP sistemi (Zorluk, Süre, Oyuncu Sayısı, Kurallar)
│   │   │   └── Article.cs                         # SEO/GEO uyumlu blog ve rehber yazısı (TopicCluster, TldrSummary, ContentMarkdown, FaqJson)
│   │   ├── Configurations/
│   │   │   ├── ClubEventConfiguration.cs          # Slug tekil indeksleri ve ilişki kuralları
│   │   │   ├── GameTableConfiguration.cs          # ConcurrencyToken (RowVersion) ile aynı anda son koltuğun çift satılmasını önleme
│   │   │   ├── BoardGameConfiguration.cs          # JSONB sütun eşlemeleri
│   │   │   └── ArticleConfiguration.cs            # Tam metin arama (GIN Index) ve Slug indeksleri
│   │   ├── Repositories/
│   │   │   ├── IGenericRepository.cs              # Temel CRUD ve IQueryable soyutlaması
│   │   │   ├── GenericRepository.cs               # EF Core 10 implementasyonu
│   │   │   └── EventRepository.cs                 # Etkinlikleri masalarıyla birlikte (Include) optimize çeken özel repository
│   │   └── DataServiceRegistration.cs             # AddDataLayer() DI kayıt uzantısı
│   │
│   └── Masabasi.Frontend/                         # [KATMAN 4] Angular 20/21 Hybrid SSR/SSG + API-Optional Ön Yüz
│       ├── public/
│       │   ├── robots.txt                         # Arama motorları ve AI botları (GPTBot, ClaudeBot, PerplexityBot) izin dosyası
│       │   ├── sitemap.xml                        # Tüm statik ve dinamik rotaların öncelikli XML haritası
│       │   ├── llms.txt                           # AI modelleri için kulüp kimliği, Cumartesi kuralları ve ana indeks (v2)
│       │   ├── llms-ctx.txt                       # AI modelleri için genişletilmiş kulüp rehberi, SSS ve oyun envanteri
│       │   └── data/                              # [KRİTİK] API Kapalıyken / Statik Modda Kullanılan Snapshot Verileri
│       │       ├── club-config.snapshot.json      # Kulüp sosyal medya linkleri, sabit buluşma mekanı ve duyuru metni
│       │       ├── events.snapshot.json           # Yaklaşan ve geçmiş Cumartesi etkinlikleri + Masa listeleri
│       │       ├── games.snapshot.json            # Tüm kutu oyunları ve FRP sistemleri kataloğu
│       │       └── articles.snapshot.json         # Blog yazıları, kural rehberleri ve SSS verileri
│       ├── src/
│       │   ├── index.html                         # Temel HTML5 şablonu, Google Fonts preload (`Cinzel` & `Plus Jakarta Sans`)
│       │   ├── main.ts                            # Tarayıcı bootstrap noktası
│       │   ├── main.server.ts                     # Sunucu (SSR/Prerender) bootstrap noktası
│       │   ├── server.ts                          # Express SSR sunucusu ve statik dosya önbellek başlıkları
│       │   ├── environments/
│       │   │   ├── environment.ts                 # apiUrl, useStaticOnly: false, apiTimeoutMs: 1500, meetupFallbackUrl
│       │   │   └── environment.static.ts          # useStaticOnly: true (Tamamen API'siz 0 TL statik hosting modu)
│       │   ├── styles/
│       │   │   ├── _variables.scss                # 60-30-10 Renk Teorisi CSS değişkenleri (Dark & Light Mode)
│       │   │   └── styles.scss                    # Global tipografi, kart gölgeleri ve erişilebilirlik stilleri
│       │   └── app/
│       │       ├── app.config.ts                  # Hydration, Fetch HttpClient ve Interceptor kayıtları
│       │       ├── app.config.server.ts           # SSR sağlayıcıları
│       │       ├── app.routes.ts                  # Public (SSR/SSG) ve Admin (Lazy CSR) rota tanımları
│       │       ├── app.routes.server.ts           # RenderMode.Prerender, RenderMode.Server ve RenderMode.Client eşlemeleri
│       │       ├── core/
│       │       │   ├── models/                    # TypeScript arayüzleri (EventModel, GameTableModel, ArticleModel)
│       │       │   ├── services/
│       │       │   │   ├── resilient-data.service.ts  # [KALP] Önce API'yi deneyen, hata/kapalı durumda /data/*.json'a düşen servis
│       │       │   │   ├── seo-meta.service.ts        # Title, Meta, Canonical ve OpenGraph yönetici servis
│       │       │   │   ├── json-ld-schema.service.ts  # Event, Organization, Article ve FAQPage JSON-LD enjektörü
│       │       │   │   └── live-table-hub.service.ts  # API aktifse SignalR'a bağlanan, değilse sessizce pasif kalan servis
│       │       │   └── guards/
│       │       │       └── admin-auth.guard.ts        # Admin paneline yetkisiz girişi engelleyen guard
│       │       ├── shared/
│       │       │   └── components/
│       │       │       ├── navbar/                    # Üst menü + Tema değiştirici + Mobil "Yerini Ayırt" sabit barı
│       │       │       ├── footer/                    # Topic Cluster mimarisine uygun temiz SEO navigasyonu
│       │       │       ├── tldr-ai-box/               # GEO (Yapay Zeka) için semantik <aside> özet kutusu bileşeni
│       │       │       └── table-seat-card/           # Masa doluluk çubuğu, DM bilgisi ve Hibrit Kayıt (API/Meetup) kartı
│       │       └── features/
│       │           ├── home/                          # Ana sayfa (Hero, Bu Cumartesi Masalar, Kulüp Tanıtımı, SSS)
│       │           ├── events/                        # /etkinlikler ve /etkinlikler/:slug (Event Schema + Masa Seçimi)
│       │           ├── games-library/                 # /oyun-kutuphanesi ve /oyunlar/:slug (Kutu Oyunu & FRP Rehberleri)
│       │           ├── blog-hub/                      # /blog ve /blog/:category/:slug (Kart, Dijital RPG, Cosplay/Anime Köprüleri)
│       │           └── admin/                         # [İZOLE CSR] /admin (Etkinlik, Masa, Oyun, Blog ve Snapshot Yönetimi)
├── tests/
│   └── Masabasi.UnitTests/                        # xUnit test projesi (EventManagerTests, StaticSnapshotManagerTests)
├── docker-compose.yml                             # PostgreSQL, Masabasi.API, Masabasi.Frontend ve Nginx orkestrasyonu
└── Masabasi.sln                                   # .NET 10 Solution dosyası
```


## BÖLÜM 5: KOD ÖRNEKLERİ VE YAZIM STANDARTLARI (ÜRETİM KALİTESİNDE İMPLEMENTASYONLAR)

Bu bölümdeki kodlar, hem `.NET 10` hem de `Angular 20/21` tarafında projenin nasıl bir standartta yazılacağını gösteren, doğrudan kopyalanıp çalıştırılabilir referans implementasyonlardır.

### 5.1. Backend (.NET 10) — Domain Entity ve EF Core 10 Fluent API Konfigürasyonu

```csharp
// Dosya: src/Masabasi.Data/Entities/ClubEvent.cs
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
```

```csharp
// Dosya: src/Masabasi.Data/Entities/GameTable.cs
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

    public ClubEvent? ClubEvent { get; set; }
    public ICollection<TableReservation> Reservations { get; set; } = new List<TableReservation>();
}
```

```csharp
// Dosya: src/Masabasi.Data/Configurations/GameTableConfiguration.cs
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Masabasi.Data.Configurations;

/// <summary>
/// GameTable varlığı için PostgreSQL Fluent API tablo ve eşzamanlılık (concurrency) yapılandırması.
/// </summary>
public sealed class GameTableConfiguration : IEntityTypeConfiguration<GameTable>
{
    public void Configure(EntityTypeBuilder<GameTable> builder)
    {
        builder.ToTable("game_tables");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GameTitle)
            .IsRequired()
            .HasMaxLength(160);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.HostOrDmName)
            .IsRequired()
            .HasMaxLength(100);

        // PostgreSQL xmin sistem sütunu ile Optimistic Concurrency (Son koltuğa aynı anda 2 kişi tıklarsa 2. kişiyi engeller)
        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => new { x.ClubEventId, x.TableNumber })
            .IsUnique();
    }
}
```

### 5.2. Backend (.NET 10) — Standart `ServiceResult<T>` ve İş Katmanı (`EventManager` & `StaticSnapshotManager`)

```csharp
// Dosya: src/Masabasi.Business/Common/ServiceResult.cs
namespace Masabasi.Business.Common;

/// <summary>
/// İş katmanı metotlarının istisna (exception) fırlatmadan standart sonuç döndürmesini sağlayan sarmalayıcı.
/// </summary>
public sealed record ServiceResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public int StatusCode { get; init; }

    public static ServiceResult<T> Success(T data, int statusCode = 200) =>
        new() { IsSuccess = true, Data = data, StatusCode = statusCode };

    public static ServiceResult<T> Failure(string errorMessage, int statusCode = 400) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };
}
```

```csharp
// Dosya: src/Masabasi.Business/Services/StaticSnapshotManager.cs
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Masabasi.Business.Interfaces;
using Masabasi.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masabasi.Business.Services;

/// <summary>
/// Sitenin API'siz (Static/Offline) modda da %100 çalışabilmesi için veritabanındaki güncel verileri
/// Angular'ın /public/data/ dizinine JSON, /public/llms.txt ve /public/sitemap.xml olarak çıkaran servis.
/// </summary>
public sealed class StaticSnapshotManager : IStaticSnapshotService
{
    private readonly MasabasiDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StaticSnapshotManager> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public StaticSnapshotManager(
        MasabasiDbContext dbContext,
        IConfiguration configuration,
        ILogger<StaticSnapshotManager> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Tüm aktif etkinlikleri, oyun kütüphanesini, blog yazılarını ve AI indeksleme dosyalarını (llms.txt) statik diske yazar.
    /// </summary>
    public async Task ExportAllSnapshotsAsync(CancellationToken cancellationToken = default)
    {
        string publicRootPath = _configuration["SnapshotSettings:FrontendPublicPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "..", "Masabasi.Frontend", "public");

        string dataDirPath = Path.Combine(publicRootPath, "data");
        Directory.CreateDirectory(dataDirPath);

        // 1. Etkinlikler ve Masaları JSON olarak dışa aktar
        var events = await _dbContext.ClubEvents
            .AsNoTracking()
            .Include(e => e.Tables.OrderBy(t => t.TableNumber))
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.StartDateUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        string eventsJson = JsonSerializer.Serialize(events, JsonOptions);
        await File.WriteAllTextAsync(
            Path.Combine(dataDirPath, "events.snapshot.json"),
            eventsJson,
            Encoding.UTF8,
            cancellationToken);

        // 2. Yapay Zeka Arama Motorları için /llms.txt (v2 Ağustos 2026 Standardı) dosyasını güncelle
        string llmsTxtContent = BuildLlmsTxtContent(events);
        await File.WriteAllTextAsync(
            Path.Combine(publicRootPath, "llms.txt"),
            llmsTxtContent,
            Encoding.UTF8,
            cancellationToken);

        _logger.LogInformation("Statik snapshot dosyaları ve llms.txt başarıyla güncellendi: {Path}", publicRootPath);
    }

    private static string BuildLlmsTxtContent(IReadOnlyList<Data.Entities.ClubEvent> recentEvents)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Masabaşı Oyun Kulübü");
        sb.AppendLine();
        sb.AppendLine("> Masabaşı Oyun Kulübü, İstanbul'da her Cumartesi düzenli olarak kutu oyunları (Board Games), sosyal çıkarım oyunları (Secret Hitler, Avalon) ve masaüstü rol yapma oyunları (FRP / D&D 5e) etkinlikleri düzenleyen topluluktur.");
        sb.AppendLine();
        sb.AppendLine("## Temel Bilgiler ve İletişim");
        sb.AppendLine("- **Etkinlik Günü:** Her Cumartesi");
        sb.AppendLine("- **Konum:** İstanbul, Türkiye");
        sb.AppendLine("- **Instagram:** https://www.instagram.com/masabasikulubu/");
        sb.AppendLine("- **Meetup:** https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/");
        sb.AppendLine();
        sb.AppendLine("## Güncel Cumartesi Etkinlikleri ve Açık Masalar");
        foreach (var ev in recentEvents.Take(3))
        {
            sb.AppendLine($"- [{ev.Title}](https://masabasikulubu.com/etkinlikler/{ev.Slug}): {ev.TldrSummary} (Mekan: {ev.VenueName}, Tarih: {ev.StartDateUtc:yyyy-MM-dd HH:mm})");
        }
        sb.AppendLine();
        sb.AppendLine("## Ana Rehberler ve Oyun Kütüphanesi");
        sb.AppendLine("- [Oyun Kütüphanemiz ve Kural Rehberleri](https://masabasikulubu.com/oyun-kutuphanesi): Kulüpte oynanan kutu oyunları ve Türkçe kuralları.");
        sb.AppendLine("- [FRP ve D&D 5e Başlangıç Rehberi](https://masabasikulubu.com/frp-dnd-rehberi): İlk kez masaüstü rol yapma oyununa katılacaklar için rehber.");
        sb.AppendLine("- [Geek Kültür & Masaüstü Köprüsü Blog](https://masabasikulubu.com/blog): Kart oyunları (MTG), Baldur's Gate 3'ten D&D'ye geçiş ve masaüstü cosplay yazıları.");
        return sb.ToString();
    }
}
```

### 5.3. Backend (.NET 10) — Admin vs. Public Trafik İzolasyonu (`Program.cs` & Rate Limiting)

```csharp
// Dosya: src/Masabasi.API/Extensions/RateLimitingExtensions.cs
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Extensions;

/// <summary>
/// Vitrin (Public) trafiğinin aşırı yüklenmesi durumunda Admin panelinin ve API'nin etkilenmemesi için
/// izole edilmiş hız sınırlama (Rate Limiting) havuzlarını yapılandırır.
/// </summary>
public static class RateLimitingExtensions
{
    public const string PublicReadPolicy = "PublicReadPolicy";
    public const string PublicReservationPolicy = "PublicReservationPolicy";
    public const string AdminIsolatedPolicy = "AdminIsolatedPolicy";

    public static IServiceCollection AddTrafficIsolationPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 1. Halka Açık Okuma Havuzu: IP başına dakikada 120 istek
            options.AddPolicy(PublicReadPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 120,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            // 2. Masa Koltuk Kayıt Havuzu (Spam Bot Koruması): IP başına 10 dakikada en fazla 5 kayıt denemesi
            options.AddPolicy(PublicReservationPolicy, httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon-post",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(10),
                        SegmentsPerWindow = 2,
                        QueueLimit = 0
                    }));

            // 3. İzole Admin Havuzu: Public havuzları tamamen dolsa bile Admin işlemleri ayrı havuzdan engelsiz çalışır
            options.AddPolicy(AdminIsolatedPolicy, httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? "admin-partition",
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 500,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 500,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }
}
```

### 5.4. Frontend (Angular 20/21) — API'li ve API'siz Çift Modlu Veri Servisi (`ResilientDataService`)

Bu servis, sitenin **API hiç yokken bile** veya **API çöktüğünde** hiçbir hata göstermeden çalışmasını sağlayan kalptir:

```typescript
// Dosya: src/Masabasi.Frontend/src/app/core/services/resilient-data.service.ts
import { Injectable, inject, signal, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { Observable, of } from 'rxjs';
import { catchError, timeout, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ClubEventModel } from '../models/event-table.model';

export type DataSourceMode = 'LIVE_API' | 'STATIC_FALLBACK';

@Injectable({ providedIn: 'root' })
export class ResilientDataService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);

  /**
   * UI bileşenlerinin şu an canlı API'den mi yoksa statik JSON snapshot'tan mı
   * beslendiğini bilmesini sağlayan reaktif Signal.
   * 'STATIC_FALLBACK' olduğunda masa kartlarındaki kayıt butonu otomatik olarak
   * Meetup / WhatsApp / Instagram DM yönlendirme moduna geçer!
   */
  public readonly activeMode = signal<DataSourceMode>(
    environment.useStaticOnly ? 'STATIC_FALLBACK' : 'LIVE_API'
  );

  /**
   * Cumartesi etkinliklerini ve masa kontenjanlarını getirir.
   * 1) Eğer environment.useStaticOnly === true ise doğrudan /data/events.snapshot.json okunur.
   * 2) Eğer API aktifse önce .NET API'ye gider; ancak 1500ms içinde cevap gelmezse
   *    veya sunucu kapalıysa sıfır hata ile /data/events.snapshot.json verisine düşer.
   */
  public getEvents(): Observable<ClubEventModel[]> {
    if (environment.useStaticOnly) {
      this.activeMode.set('STATIC_FALLBACK');
      return this.http.get<ClubEventModel[]>('/data/events.snapshot.json');
    }

    return this.http
      .get<ClubEventModel[]>(`${environment.apiUrl}/api/v1/public/events`)
      .pipe(
        timeout(environment.apiTimeoutMs ?? 1500),
        tap(() => this.activeMode.set('LIVE_API')),
        catchError((err) => {
          if (isPlatformBrowser(this.platformId)) {
            console.warn(
              '[Masabaşı Resilient Mode] .NET API erişilemez durumda, statik snapshot verisine geçildi.',
              err?.message
            );
          }
          this.activeMode.set('STATIC_FALLBACK');
          return this.http.get<ClubEventModel[]>('/data/events.snapshot.json');
        })
      );
  }
}
```

### 5.5. Frontend (Angular 20/21) — Rota Bazlı Hibrit Render (`app.routes.server.ts`)

Googlebot ve yapay zeka botlarının tüm sayfaları tam HTML olarak okuması, Admin panelinin ise tamamen izole CSR olması için Angular sunucu rota yapılandırması:

```typescript
// Dosya: src/Masabasi.Frontend/src/app/app.routes.server.ts
import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // 1. Ana sayfa, Hakkımızda, Oyun Kütüphanesi ve Blog yazıları derleme anında (SSG) statik HTML'e çevrilir.
  // Sunucuya sıfır yük bindirir, TTFB < 30ms olur ve AI botları tarafından anında okunur.
  {
    path: '',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'hakkimizda',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'oyun-kutuphanesi',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'oyunlar/:slug',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'frp-dnd-rehberi',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'blog/:category/:slug',
    renderMode: RenderMode.Prerender,
  },

  // 2. Cumartesi Etkinlikleri sayfası hem Prerender (SSG) hem SSR destekler
  {
    path: 'etkinlikler',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'etkinlikler/:slug',
    renderMode: RenderMode.Prerender,
  },

  // 3. İZOLE ADMİN PANELİ: Sadece tarayıcıda (CSR) çalışır, arama motoru indeksine girmez.
  {
    path: 'admin/**',
    renderMode: RenderMode.Client,
  },

  // Diğer tüm rotalar için varsayılan
  {
    path: '**',
    renderMode: RenderMode.Prerender,
  },
];
```

### 5.6. Frontend (Angular 20/21) — Yapay Zeka ve Google İçin `JsonLdSchemaService`

```typescript
// Dosya: src/Masabasi.Frontend/src/app/core/services/json-ld-schema.service.ts
import { Injectable, inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { ClubEventModel } from '../models/event-table.model';

@Injectable({ providedIn: 'root' })
export class JsonLdSchemaService {
  private readonly doc = inject(DOCUMENT);

  /**
   * Sayfanın <head> bölümüne Schema.org JSON-LD script etiketi ekler.
   * Hem SSR/Prerender sırasında sunucuda hem de tarayıcıda çalışır.
   */
  public setSchema(schemaId: string, schemaObject: Record<string, unknown>): void {
    const existing = this.doc.getElementById(schemaId);
    if (existing) {
      existing.textContent = JSON.stringify(schemaObject);
      return;
    }

    const script = this.doc.createElement('script');
    script.id = schemaId;
    script.type = 'application/ld+json';
    script.textContent = JSON.stringify(schemaObject);
    this.doc.head.appendChild(script);
  }

  /**
   * Cumartesi kutu oyunu ve FRP etkinlikleri için Google Event Rich Result şeması üretir.
   */
  public setSaturdayEventSchema(event: ClubEventModel): void {
    this.setSchema('schema-saturday-event', {
      '@context': 'https://schema.org',
      '@type': 'SocialEvent',
      name: event.title,
      description: event.tldrSummary,
      startDate: event.startDateUtc,
      endDate: event.endDateUtc,
      eventStatus: 'https://schema.org/EventScheduled',
      eventAttendanceMode: 'https://schema.org/OfflineEventAttendanceMode',
      isAccessibleForFree: false,
      location: {
        '@type': 'Place',
        name: event.venueName,
        address: {
          '@type': 'PostalAddress',
          streetAddress: event.venueAddress,
          addressLocality: 'İstanbul',
          addressCountry: 'TR',
        },
      },
      organizer: {
        '@type': 'Organization',
        name: 'Masabaşı Oyun Kulübü',
        url: 'https://masabasikulubu.com',
        sameAs: [
          'https://www.instagram.com/masabasikulubu/',
          'https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/',
          'https://www.facebook.com/groups/14080585061/user/61595360127902/',
        ],
      },
    });
  }
}
```

### 5.7. 60-30-10 Renk Teorisi SCSS Tasarım Sistemi (`_variables.scss`)

```scss
// Dosya: src/Masabasi.Frontend/src/styles/_variables.scss
// ============================================================================
// MASABAŞI OYUN KULÜBÜ — 60-30-10 RENK TEORİSİ TASARIM TOKENLARI
// ============================================================================

:root {
  // --- VARSAYILAN TEMA: KOYU ("TAVERNA & ZİNDAN" ATMOSFERİ) ---

  // %60 DOMİNANT: Arka Plan ve Geniş Okuma Yüzeyleri
  --bg-dominant-primary: #121619;     // Derin Arduvaz / Gece Kömürü
  --bg-dominant-elevated: #1A2126;    // Koyu Çam Yüzeyi
  --text-primary: #F4EFE6;            // Sıcak Parşömen Kremi (WCAG AAA: 13.8:1 kontrast)
  --text-muted: #B8B0A4;              // Küllü Parşömen (İkincil metinler)

  // %30 SEKONDER: Kulüp Marka Kimliği (Logodaki ve Mekandaki Yeşil & Ceviz Ahşap)
  --surface-secondary-emerald: #1E3A34; // Masabaşı Zümrüt / Adaçayı Yeşili
  --surface-secondary-emerald-hover: #2A524A;
  --surface-secondary-walnut: #2C221E;  // Ceviz Masa Ahşabı (Masa Kartları)
  --border-secondary-subtle: #2F4F46;

  // %10 VURGU (ACCENT): Aksiyon Butonları, Logo Altını ve D20 Kritik Zar Kızılı
  --accent-gold-primary: #D4AF37;       // Logo Pirinç / Altın Halkası
  --accent-gold-hover: #E5C158;
  --accent-gold-contrast-text: #121619; // Altın buton üzerindeki koyu metin
  --accent-crimson-d20: #E05A47;        // Kritik Zar (Nat 20) Kızılı ("Son 1 Koltuk!")
  --accent-success-seat: #34D399;       // Boş koltuk göstergesi

  // Tipografi
  --font-display: 'Cinzel', Georgia, serif;
  --font-sans: 'Plus Jakarta Sans', system-ui, -apple-system, sans-serif;
}

// --- AÇIK TEMA ("PARŞÖMEN & GÜNDÜZ KAFE" OKUMA MODU) ---
[data-theme='parchment'] {
  // %60 DOMİNANT
  --bg-dominant-primary: #FAF6F0;     // Sıcak Parşömen Kremi
  --bg-dominant-elevated: #F0E9DF;    // Yumuşak Keten
  --text-primary: #1C2226;            // Koyu Mürekkep
  --text-muted: #525B62;

  // %30 SEKONDER
  --surface-secondary-emerald: #264E46;
  --surface-secondary-emerald-hover: #1E3A34;
  --surface-secondary-walnut: #EAE0D2;
  --border-secondary-subtle: #D8CBB8;

  // %10 VURGU
  --accent-gold-primary: #B8860B;
  --accent-gold-hover: #9A7009;
  --accent-gold-contrast-text: #FFFFFF;
  --accent-crimson-d20: #C84B31;
}
```



### 5.8. Backend (.NET 10) — `EventManager.cs` (Koltuk Kontenjanı, Overbooking Koruması ve Otomatik Snapshot Tetikleme)

```csharp
// Dosya: src/Masabasi.Business/Services/EventManager.cs
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
    private readonly ILogger<EventManager> _logger;

    private const string ActiveEventsCacheKey = "public:events:active-list";

    public EventManager(
        MasabasiDbContext dbContext,
        IStaticSnapshotService snapshotService,
        HybridCache hybridCache,
        ILogger<EventManager> logger)
    {
        _dbContext = dbContext;
        _snapshotService = snapshotService;
        _hybridCache = hybridCache;
        _logger = logger;
    }

    /// <summary>
    /// Halka açık vitrin için yaklaşan Cumartesi etkinliklerini önbellekten (HybridCache) getirir.
    /// Vitrine binlerce kişi girdiğinde PostgreSQL'e sorgu gitmesini engeller.
    /// </summary>
    public async Task<ServiceResult<IReadOnlyList<EventDetailDto>>> GetActiveEventsCachedAsync(
        CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Bir oyuncunun seçtiği masaya (Kutu Oyunu veya FRP) koltuk kaydı oluşturmasını sağlar.
    /// Optimistic Concurrency (RowVersion) ile son koltuğun aynı anda iki kişiye verilmesini engeller.
    /// </summary>
    public async Task<ServiceResult<ReservationResultDto>> ReserveTableSeatAsync(
        CreateReservationDto request,
        CancellationToken cancellationToken = default)
    {
        var table = await _dbContext.GameTables
            .Include(t => t.ClubEvent)
            .FirstOrDefaultAsync(t => t.Id == request.GameTableId && !t.IsDeleted, cancellationToken);

        if (table is null || table.ClubEvent is null || !table.ClubEvent.IsActive)
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
            Notes = request.Notes,
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

        // Önbelleği temizle ve statik JSON snapshot dosyasını arka planda güncelle
        await _hybridCache.RemoveAsync(ActiveEventsCacheKey, cancellationToken);
        await _snapshotService.ExportAllSnapshotsAsync(cancellationToken);

        return ServiceResult<ReservationResultDto>.Success(new ReservationResultDto(
            ReservationId: reservation.Id,
            TableId: table.Id,
            TableNumber: table.TableNumber,
            GameTitle: table.GameTitle,
            CurrentPlayers: table.CurrentPlayers,
            MaxPlayers: table.MaxPlayers));
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
}
```

### 5.9. Frontend (Angular 20/21) — Hibrit Masa Kartı Bileşeni (`TableSeatCardComponent`)

Bu bileşen, **60-30-10 renk kuralını** uygular ve `ResilientDataService.activeMode()` değerine göre **API açıkken siteden anlık koltuk kaydı**, **API kapalıyken ise Meetup / Instagram / WhatsApp üzerinden kayıt** seçeneğini otomatik sunar:

```typescript
// Dosya: src/Masabasi.Frontend/src/app/shared/components/table-seat-card/table-seat-card.component.ts
import { Component, ChangeDetectionStrategy, input, output, computed, inject } from '@angular/core';
import { GameTableModel } from '../../../core/models/event-table.model';
import { ResilientDataService } from '../../../core/services/resilient-data.service';

@Component({
  selector: 'app-table-seat-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <article
      class="table-card"
      [class.table-card--full]="isFull()"
      [attr.aria-label]="'Masa ' + table().tableNumber + ' - ' + table().gameTitle">
      
      <!-- Üst Bilgi: Masa Numarası ve Kategori Rozeti (%30 Sekonder Yüzey) -->
      <header class="table-card__header">
        <span class="table-card__number">Masa #{{ table().tableNumber }}</span>
        <span class="table-card__badge">{{ categoryLabel() }}</span>
        @if (table().isBeginnerFriendly) {
          <span class="table-card__beginner-tag">Yeni Başlayan Dostu</span>
        }
      </header>

      <!-- Oyun / FRP Senaryo Başlığı ve DM Bilgisi -->
      <div class="table-card__body">
        <h3 class="table-card__title">{{ table().gameTitle }}</h3>
        <p class="table-card__host">
          <strong>Masa Yöneticisi / DM:</strong> {{ table().hostOrDmName }}
        </p>

        <!-- Koltuk Doluluk Çubuğu -->
        <div class="table-card__capacity">
          <div class="table-card__capacity-labels">
            <span>Kontenjan Durumu</span>
            <strong [class.text-crimson]="remainingSeats() === 1">
              {{ table().currentPlayers }} / {{ table().maxPlayers }} Oyuncu
              @if (remainingSeats() === 1) {
                (Son 1 Koltuk!)
              }
            </strong>
          </div>
          <div class="table-card__progress-track" role="progressbar"
               [attr.aria-valuenow]="table().currentPlayers"
               [attr.aria-valuemin]="0"
               [attr.aria-valuemax]="table().maxPlayers">
            <div class="table-card__progress-fill" [style.width.%]="fillPercentage()"></div>
          </div>
        </div>
      </div>

      <!-- %10 Vurgu (Accent Gold) Hibrit Aksiyon Alanı -->
      <footer class="table-card__footer">
        @if (isFull()) {
          <button type="button" class="btn-disabled" disabled>
            Masa Kontenjanı Doldu
          </button>
        } @else if (dataService.activeMode() === 'LIVE_API') {
          <!-- Durum 1: .NET API Aktif -> Doğrudan Siteden Koltuk Ayırt -->
          <button type="button" class="btn-accent-gold" (click)="reserveClicked.emit(table())">
            Masada Yerini Ayırt ({{ remainingSeats() }} Boş Koltuk)
          </button>
        } @else {
          <!-- Durum 2: API Kapalı / Statik Mod -> Meetup veya Instagram üzerinden sıfır hatayla yönlendir -->
          <a [href]="fallbackRegistrationUrl()"
             target="_blank"
             rel="noopener noreferrer"
             class="btn-accent-gold">
            Meetup / Instagram ile Masaya Katıl
          </a>
        }
      </footer>
    </article>
  `,
  styles: [`
    .table-card {
      background-color: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 12px;
      padding: 1.25rem;
      color: var(--text-primary);
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      gap: 1rem;
      transition: transform 0.2s ease, border-color 0.2s ease;
    }
    .table-card:hover {
      transform: translateY(-2px);
      border-color: var(--accent-gold-primary);
    }
    .table-card__header {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      gap: 0.5rem;
    }
    .table-card__number {
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
      font-family: var(--font-display);
      font-weight: 700;
      padding: 0.25rem 0.65rem;
      border-radius: 6px;
      font-size: 0.875rem;
    }
    .table-card__title {
      font-family: var(--font-display);
      font-size: 1.25rem;
      margin: 0.25rem 0;
      color: var(--text-primary);
    }
    .btn-accent-gold {
      display: inline-flex;
      justify-content: center;
      align-items: center;
      width: 100%;
      background-color: var(--accent-gold-primary);
      color: var(--accent-gold-contrast-text);
      font-weight: 700;
      padding: 0.75rem 1rem;
      border-radius: 8px;
      text-decoration: none;
      border: none;
      cursor: pointer;
    }
    .btn-accent-gold:hover {
      background-color: var(--accent-gold-hover);
    }
    .text-crimson {
      color: var(--accent-crimson-d20);
    }
  `]
})
export class TableSeatCardComponent {
  public readonly dataService = inject(ResilientDataService);

  public readonly table = input.required<GameTableModel>();
  public readonly meetupUrl = input<string>(
    'https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/'
  );
  public readonly reserveClicked = output<GameTableModel>();

  public readonly remainingSeats = computed(() =>
    Math.max(0, this.table().maxPlayers - this.table().currentPlayers)
  );

  public readonly isFull = computed(() => this.remainingSeats() === 0);

  public readonly fillPercentage = computed(() =>
    Math.min(100, Math.round((this.table().currentPlayers / this.table().maxPlayers) * 100))
  );

  public readonly categoryLabel = computed(() => {
    const map: Record<string, string> = {
      BoardGame: 'Kutu Oyunu',
      SocialDeduction: 'Sosyal Çıkarım & Parti',
      FRP_DnD: 'FRP / D&D 5e',
      CardGame: 'Kart Oyunu (TCG)'
    };
    return map[this.table().category] ?? 'Masaüstü Oyun';
  });

  public readonly fallbackRegistrationUrl = computed(() => this.meetupUrl());
}
```

### 5.10. Statik Snapshot Örneği (`/public/data/events.snapshot.json`) ve Nginx İzolasyon Konfigürasyonu (`nginx/nginx.conf`)

API kapalıyken veya tamamen ücretsiz statik hosting kullanıldığında Angular'ın okuduğu `/public/data/events.snapshot.json` dosyasının veri şeması:

```json
[
  {
    "id": "8f9d4a10-6b2c-4e11-9d4b-159a01b2c3d4",
    "title": "Masabaşı Cumartesi Buluşması: Kutu Oyunları & D&D One-Shot Gecesi",
    "slug": "masabasi-cumartesi-bulusmasi-kutu-oyunlari-ve-dnd",
    "tldrSummary": "Masabaşı Oyun Kulübü olarak bu Cumartesi İstanbul'da kutu oyunları (Secret Hitler, Catan, Avalon) ve yeni başlayanlara uygun D&D 5e FRP masaları için buluşuyoruz. Oyunları bilmenize gerek yoktur, kurallar masada anlatılmaktadır.",
    "startDateUtc": "2026-10-17T11:00:00Z",
    "endDateUtc": "2026-10-17T18:00:00Z",
    "venueName": "Masabaşı Buluşma Noktası - İstanbul",
    "venueAddress": "Kadıköy / İstanbul",
    "meetupEventUrl": "https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/events/316644892/",
    "externalRegistrationUrl": "https://www.instagram.com/masabasikulubu/",
    "isActive": true,
    "tables": [
      {
        "id": "12a00001-0000-0000-0000-000000000012",
        "tableNumber": 12,
        "gameTitle": "Secret Hitler (Kavgam) & Avalon — Sosyal Çıkarım Masası",
        "category": "SocialDeduction",
        "hostOrDmName": "Masabaşı Ekibi",
        "maxPlayers": 10,
        "currentPlayers": 8,
        "isBeginnerFriendly": true
      },
      {
        "id": "12a00002-0000-0000-0000-000000000002",
        "tableNumber": 2,
        "gameTitle": "Dungeons & Dragons 5e: Zindanların Çağrısı (One-Shot)",
        "category": "FRP_DnD",
        "hostOrDmName": "DM Kerem",
        "maxPlayers": 5,
        "currentPlayers": 4,
        "isBeginnerFriendly": true
      }
    ]
  }
]
```

Vitrin trafiği ile Admin trafiğini sunucu seviyesinde birbirinden ayıran `nginx/nginx.conf` yapılandırması:

```nginx
# Dosya: nginx/nginx.conf
worker_processes auto;

events {
    worker_connections 4096;
}

http {
    include       mime.types;
    default_type  application/octet-stream;
    sendfile      on;
    gzip          on;
    gzip_types    text/plain text/css application/json application/javascript text/xml application/xml+rss;

    # Halka açık API yanıtları için mikro-önbellek (Vitrine 50.000 kişi girse bile .NET API'ye saniyede sadece 1 istek gider)
    proxy_cache_path /var/cache/nginx/public_api levels=1:2 keys_zone=PUBLIC_API_CACHE:10m max_size=200m inactive=10m;

    # Rate Limiting Bölgeleri (Public vs Admin ayrımı)
    limit_req_zone $binary_remote_addr zone=public_zone:10m rate=30r/s;
    limit_req_zone $binary_remote_addr zone=admin_zone:10m rate=10r/s;

    server {
        listen 80;
        server_name masabasikulubu.com www.masabasikulubu.com;

        # 1. Yapay Zeka İndeksleme Dosyaları (llms.txt, robots.txt, sitemap.xml) ve Statik Snapshot JSON'lar
        location ~ ^/(llms\.txt|llms-ctx\.txt|robots\.txt|sitemap\.xml|data/.*\.json)$ {
            root /usr/share/nginx/html/browser;
            add_header Cache-Control "public, max-age=300, stale-while-revalidate=86400";
            try_files $uri =404;
        }

        # 2. Halka Açık API Havuzu (Mikro-Önbellekli + Hata Anında Statik JSON Fallback!)
        location /api/v1/public/ {
            limit_req zone=public_zone burst=50 nodelay;
            proxy_cache PUBLIC_API_CACHE;
            proxy_cache_valid 200 15s;
            proxy_cache_use_stale error timeout updating http_500 http_502 http_503 http_504;
            proxy_pass http://masabasi-api:8080;
        }

        # 3. İZOLE ADMİN API HAVUZU (Önbelleksiz, Ayrı Kuyruk, Doğrudan Erişim)
        location /api/v1/admin/ {
            limit_req zone=admin_zone burst=20 nodelay;
            proxy_no_cache 1;
            proxy_cache_bypass 1;
            proxy_pass http://masabasi-api:8080;
        }

        # 4. SignalR Canlı Masa Bildirim Hub'ı
        location /hubs/table {
            proxy_pass http://masabasi-api:8080;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection "upgrade";
        }

        # 5. Angular SSR / Prerendered Statik Vitrin Sayfaları
        location / {
            root /usr/share/nginx/html/browser;
            try_files $uri $uri/index.html @angular_ssr;
        }

        location @angular_ssr {
            proxy_pass http://masabasi-frontend-ssr:4000;
        }
    }
}
```

## BÖLÜM 6: FAZLARA VE ADIMLARA AYRILMIŞ UÇTAN UCA PROJE GELİŞTİRME PLANI

Bir yazılım geliştiricinin veya yapay zeka kodlama ajanının (AI Agent) bu projeyi sıfırdan alıp canlıya çıkarırken izleyeceği **6 Fazlı Sıralı Uygulama Planı** aşağıdadır. Her faz tamamlanmadan bir sonrakine geçilmemelidir.

### FAZ 1: Çözüm İskeleti, Veritabanı ve Veri Erişim Katmanı (`Masabasi.Data`)
* **Adım 1.1 — Solution ve Projelerin Oluşturulması:**
  * Kök dizinde `Masabasi.sln` oluşturulur.
  * `src/Masabasi.Data` (`Class Library`), `src/Masabasi.Business` (`Class Library`), `src/Masabasi.API` (`ASP.NET Core Web API`) ve `tests/Masabasi.UnitTests` (`xUnit`) projeleri `.NET 10.0` hedefiyle açılır.
* **Adım 1.2 — Entity ve Fluent API Konfigürasyonlarının Yazılması:**
  * `BaseEntity`, `ClubEvent`, `GameTable`, `TableReservation`, `BoardGame` ve `Article` varlıkları oluşturulur.
  * `GameTable` üzerinde aynı koltuğun iki kişiye satılmasını önleyen `RowVersion` (`xmin`) eşzamanlılık kilidi ve `ClubEvent.Slug` / `Article.Slug` üzerinde `IsUnique()` indeksleri tanımlanır.
* **Adım 1.3 — Generic Repository ve Seed Data (Başlangıç Verisi):**
  * `IGenericRepository<T>` ve `EventRepository` yazılır.
  * Kulübün gerçek başlangıç verilerini (Örn: *Secret Hitler / Kavgam*, *Catan*, *D&D 5e One-Shot* masaları ve örnek Cumartesi etkinliği) içeren `MasabasiDbSeeder` hazırlanır.

### FAZ 2: İş Mantığı, Doğrulama ve Statik Snapshot Motoru (`Masabasi.Business`)
* **Adım 2.1 — `ServiceResult<T>` ve DTO'ların İnşası:**
  * Tüm girdi ve çıktılar için immutable `record` tabanlı DTO'lar (`EventDetailDto`, `GameTableDto`, `CreateReservationDto`, `ArticleDetailDto`) yazılır.
* **Adım 2.2 — `FluentValidation` Kuralları:**
  * Masa kaydı için telefon/Instagram kullanıcı adı doğrulaması; blog ve oyun rehberleri için SEO kriterleri (`Title` 30–65 karakter, `MetaDescription` 110–160 karakter, `TldrSummary` 40–65 kelime zorunluluğu) validator sınıflarına eklenir.
* **Adım 2.3 — `EventManager` ve `StaticSnapshotManager` İmplementasyonu:**
  * `EventManager` içinde koltuk kaydı yapılırken `CurrentPlayers < MaxPlayers` kontrolü ve DB transaction yönetimi yapılır.
  * `StaticSnapshotManager` yazılır: Her admin değişikliğinde `/public/data/*.snapshot.json`, `/public/llms.txt` ve `/public/sitemap.xml` dosyalarını otomatik üreten metotlar tamamlanır ve birim testleri (`StaticSnapshotManagerTests`) yazılır.

### FAZ 3: API Geçidi, SignalR ve Trafik İzolasyonu (`Masabasi.API`)
* **Adım 3.1 — Public ve Admin Controller Ayrımı:**
  * `Controllers/Public/` altındaki uçlara `[OutputCache(Duration = 30)]` ve `[EnableRateLimiting(RateLimitingExtensions.PublicReadPolicy)]` eklenir.
  * `Controllers/Admin/` altındaki uçlara `[Authorize(Roles = "Admin,DungeonMaster")]` ve `[EnableRateLimiting(RateLimitingExtensions.AdminIsolatedPolicy)]` eklenir.
* **Adım 3.2 — `TableHub` (SignalR) Entegrasyonu:**
  * Bir masaya yeni oyuncu katıldığında veya Admin masa kontenjanını değiştirdiğinde `Clients.All.SendAsync("TableSeatUpdated", tableId, currentPlayers, maxPlayers)` olayı tetiklenir.
* **Adım 3.3 — Global Exception Middleware ve Health Check:**
  * `/health` ucu eklenir (Angular ön yüzünün API'nin açık olup olmadığını 200ms içinde anlaması için).

### FAZ 4: Angular 20/21 Hybrid SSR/SSG Kurulumu ve 60-30-10 Tasarım Sistemi (`Masabasi.Frontend`)
* **Adım 4.1 — Angular SSR ve Incremental Hydration Kurulumu:**
  * `ng new masabasi-frontend --ssr --style=scss` ile proje oluşturulur.
  * `app.config.ts` içine `provideClientHydration(withIncrementalHydration())` ve `provideHttpClient(withFetch())` eklenir.
* **Adım 4.2 — 60-30-10 Renk Paleti ve Tipografi Uygulaması:**
  * `_variables.scss` içine Taverna Koyu Teması (`#121619` / `#1E3A34` / `#D4AF37`) ve Parşömen Açık Teması değişkenleri tanımlanır.
  * `NavbarComponent`, `FooterComponent`, `TldrAiBoxComponent` ve `TableSeatCardComponent` bileşenleri geliştirilir.
* **Adım 4.3 — `ResilientDataService` (API + Statik JSON Fallback) Bağlantısı:**
  * Ön yüz servisleri yazılır. API kapalıyken (`docker stop masabasi-api`) sitenin tüm sayfalarının `/public/data/*.snapshot.json` üzerinden sıfır hatayla çalıştığı ve masa kartlarının otomatik olarak Meetup/WhatsApp kayıt moduna geçtiği doğrulanır.

### FAZ 5: SEO, GEO (Yapay Zeka İndeksleme) ve Topic Cluster İçerik Sayfaları
* **Adım 5.1 — JSON-LD Schema ve Meta Servislerinin Bağlanması:**
  * Her sayfa geçişinde `SeoMetaService` ve `JsonLdSchemaService` çalışarak `SocialEvent`, `Organization`, `Article` ve `FAQPage` yapısal verilerini `<head>` içine gömer.
* **Adım 5.2 — Topic Cluster (Köprü İçerik) Rotalarının Oluşturulması:**
  * `/oyun-kutuphanesi` ve `/oyunlar/:slug` (Kutu oyunu kural rehberleri).
  * `/frp-dnd-rehberi` (D&D 5e başlangıç rehberi ve DM tavsiyeleri).
  * `/blog/:category/:slug` altında yan alan köprü kategorileri açılır:
    * `kart-oyunlari` (*Magic: The Gathering*, *Yu-Gi-Oh!*, *Secret Hitler* stratejileri)
    * `dijitalden-masaustune` (*Baldur's Gate 3* ve *Witcher* oyuncular제 için gerçek masa FRP rehberleri)
    * `cosplay-ve-aksesuar` (Masaüstü karakter canlandırma, LARP, zar kuleleri ve minyatür boyama)
* **Adım 5.3 — İzole Admin Panelinin (`/admin`) Yazılması:**
  * Lazy-loaded CSR modunda çalışan Admin ekranında; Cumartesi etkinliği açma, masa ve DM ekleme, Markdown blog editörü ve **"Statik Snapshot Paketini Güncelle"** butonu tamamlanır.

### FAZ 6: Konteynerizasyon, Nginx İzolasyonu ve Canlıya Geçiş (Deployment)
* **Adım 6.1 — Docker ve Nginx Kurulumu:**
  * `docker-compose.yml` içinde PostgreSQL 17, `.NET 10 API`, `Angular SSR` ve `Nginx` yapılandırılır.
  * `nginx.conf` üzerinde `/api/v1/public/*` için mikro-önbellek (`proxy_cache_valid 200 10s`) ve `/api/v1/admin/*` için öncelikli geçiş tanımlanır.
* **Adım 6.2 — Esnek Dağıtım (Hem Ücretsiz Statik Hem Full-Stack):**
  * İhtiyaca göre `npm run build:static` komutuyla tamamen API'siz bir çıktı alınıp **Cloudflare Pages / GitHub Pages (0 TL maliyet)** üzerine atılabilir veya tam Docker/Cloud kurulumu ile API + DB aktif şekilde yayınlanabilir.
