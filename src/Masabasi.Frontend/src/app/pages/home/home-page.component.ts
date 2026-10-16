import { Component, ChangeDetectionStrategy, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';
import { environment } from '../../../environments/environment';

interface GameCategoryShowcase {
  id: string;
  icon: string;
  badge: string;
  title: string;
  games: string[];
  description: string;
  detailLinkText: string;
  detailRoute: string;
}

interface StepItem {
  number: string;
  title: string;
  description: string;
  highlight: string;
}

interface FaqItem {
  id: string;
  question: string;
  answer: string;
}

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="home-page">
      <!-- 1. Hero Karşılama Bölümü -->
      <section class="home-hero" aria-labelledby="hero-title">
        <div class="container home-hero__container">
          <div class="home-hero__badge">
            <span class="home-hero__badge-dot" aria-hidden="true"></span>
            <span>Kadıköy / İstanbul • Her Cumartesi Yüz Yüze Masaüstü Oyun Buluşmaları</span>
          </div>

          <h1 id="hero-title" class="home-hero__title">
            MASABAŞI OYUN KULÜBÜ
          </h1>

          <p class="home-hero__lead">
            Ekranları kapatıyoruz, zarları atıyoruz. Kadıköy'ün merkezinde modern kutu oyunları,
            FRP maceraları ve samimi dostluklar için aynı masada buluşuyoruz.
          </p>

          <p class="home-hero__sublead">
            İster tek başınıza gelin, ister arkadaşlarınızla; kuralları her zaman masada sabırla anlatıyoruz.
            Burada herkese açık bir sandalye var.
          </p>

          <div class="home-hero__actions">
            <a
              id="hero-join-meetup"
              [href]="meetupUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="btn btn--primary">
              <span class="btn__icon" aria-hidden="true">🤝</span>
              <span>Meetup ile Topluluğa Katıl</span>
            </a>

            <a
              id="hero-instagram-follow"
              [href]="instagramUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="btn btn--secondary">
              <span class="btn__icon" aria-hidden="true">📸</span>
              <span>Instagram'da Takip Et</span>
            </a>

            <a
              id="hero-view-events"
              routerLink="/etkinlikler"
              class="btn btn--ghost">
              <span class="btn__icon" aria-hidden="true">📅</span>
              <span>Haftalık Buluşma Detayları</span>
            </a>
          </div>

          <!-- Hızlı Vurgular / İstatistik Kartları -->
          <div class="home-hero__stats">
            <div class="stat-card">
              <span class="stat-card__icon" aria-hidden="true">🎲</span>
              <div class="stat-card__body">
                <strong>50+ Modern Kutu Oyunu</strong>
                <span>Catan, Avalon, Terraforming Mars, Ticket to Ride ve fazlası</span>
              </div>
            </div>

            <div class="stat-card">
              <span class="stat-card__icon" aria-hidden="true">⚔️</span>
              <div class="stat-card__body">
                <strong>D&D 5e & FRP Masaları</strong>
                <span>Yeni başlayanlara açık, DM anlatımlı fantastik maceralar</span>
              </div>
            </div>

            <div class="stat-card">
              <span class="stat-card__icon" aria-hidden="true">📍</span>
              <div class="stat-card__body">
                <strong>Kadıköy / İstanbul</strong>
                <span>Vapur, metro ve metrobüse yakın, ferah ve güvenli mekanlar</span>
              </div>
            </div>

            <div class="stat-card">
              <span class="stat-card__icon" aria-hidden="true">🤝</span>
              <div class="stat-card__body">
                <strong>%100 Yeni Başlayan Dostu</strong>
                <span>Önceden oyun bilmenize gerek yok, kurallar masada öğretilir</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 2. Kulüp Hakkında & Kuruluş Amacı -->
      <section class="section about-section" aria-labelledby="about-heading">
        <div class="container">
          <div class="section-header">
            <span class="section-eyebrow">Biz Kimiz & Ne Yapıyoruz?</span>
            <h2 id="about-heading" class="section-title">Masabaşı Oyun Kulübü'nün Kuruluş Amacı</h2>
            <p class="section-subtitle">
              Sadece kutu oyunu oynamıyoruz; dijital ekranların yorduğu zihinleri tazeleyen,
              yüz yüze iletişimi ve samimi dostlukları yeniden canlandıran bağımsız bir topluluğuz.
            </p>
          </div>

          <div class="about-grid">
            <article class="about-card">
              <div class="about-card__icon-wrapper" aria-hidden="true">
                <span class="about-card__icon">📵</span>
              </div>
              <h3 class="about-card__title">Dijitalden Masaya Dönüş</h3>
              <p class="about-card__text">
                Telefonların ve bilgisayar ekranlarının esiri olduğumuz günümüz dünyasında,
                telefonları masanın kenarına bırakıp birbiriyle göz teması kuran, sohbet eden ve
                aynı masada gülen insanları bir araya getirmek için kurulduk.
              </p>
            </article>

            <article class="about-card">
              <div class="about-card__icon-wrapper" aria-hidden="true">
                <span class="about-card__icon">🌱</span>
              </div>
              <h3 class="about-card__title">Yeni Başlayanlara Kucak Açma</h3>
              <p class="about-card__text">
                "Ben kutu oyunu bilmiyorum", "D&D zarlarını nasıl atacağımı bilmiyorum", "Oynayanlar çok tecrübelidir,
                aralarına giremem" gibi çekinceleri tamamen ortadan kaldırıyoruz. Her masada sabırla kural anlatan
                moderatörlerimiz ve gönüllülerimiz vardır.
              </p>
            </article>

            <article class="about-card">
              <div class="about-card__icon-wrapper" aria-hidden="true">
                <span class="about-card__icon">🛡️</span>
              </div>
              <h3 class="about-card__title">Kapsayıcı, Güvenli ve Saygılı Ortam</h3>
              <p class="about-card__text">
                Kapımızdan adım atan herkes için zorbalıktan, üstenci tavırlardan uzak; nezaket,
                saygı ve karşılıklı hoşgörünün esas olduğu güvenli bir sosyal oyun alanı sunuyoruz.
                Burada herkes eşittir ve eğlence ortaktır.
              </p>
            </article>
          </div>
        </div>
      </section>

      <!-- 3. Nerede ve Ne Zaman Buluşuyoruz? -->
      <section class="section venue-section" aria-labelledby="venue-heading">
        <div class="container">
          <div class="venue-card">
            <div class="venue-card__content">
              <span class="section-eyebrow">Buluşma Noktamız</span>
              <h2 id="venue-heading" class="venue-card__title">Nerede ve Ne Zaman Buluşuyoruz?</h2>
              <p class="venue-card__lead">
                İstanbul'un en hareketli ve merkezi kültür noktası olan <strong>Kadıköy</strong>'de toplanıyoruz.
              </p>

              <div class="venue-details">
                <div class="venue-item">
                  <span class="venue-item__icon" aria-hidden="true">📍</span>
                  <div>
                    <strong>Konum & Ulaşım</strong>
                    <p>Kadıköy / İstanbul (Moda ve Rıhtım çevresi). Metro, metrobüs, vapur ve Marmaray hatlarına birkaç dakikalık yürüme mesafesinde.</p>
                  </div>
                </div>

                <div class="venue-item">
                  <span class="venue-item__icon" aria-hidden="true">⏰</span>
                  <div>
                    <strong>Buluşma Zamanı</strong>
                    <p>Her Cumartesi günü saat 11:00'de başlar, akşam 18:00 ve sonrasına kadar gün boyu devam eder. Dilediğiniz saatte katılıp ayrılabilirsiniz.</p>
                  </div>
                </div>

                <div class="venue-item">
                  <span class="venue-item__icon" aria-hidden="true">☕</span>
                  <div>
                    <strong>Mekan ve Atmosfer</strong>
                    <p>Masaüstü oyunlarına özel geniş masalara sahip, çay-kahve ve yemek servisi bulunan, aydınlık, ferah ve nezih kafeler/mekanlar tercih edilir.</p>
                  </div>
                </div>
              </div>

              <div class="venue-card__cta">
                <a
                  [href]="meetupUrl"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="btn btn--primary">
                  <span>Bu Cumartesi Buluşmasını İncele</span>
                </a>
              </div>
            </div>

            <div class="venue-card__side">
              <div class="quote-box">
                <div class="quote-box__emblem" aria-hidden="true">🎲</div>
                <blockquote class="quote-box__quote">
                  "Masabaşı sadece kutu oyunları oynanan bir yer değil; yalnız hissettiğiniz bir Cumartesi gününde
                  aynı masayı paylaşıp kahkahalarla dostluk kurabileceğiniz sıcacık bir limandır."
                </blockquote>
                <cite class="quote-box__author">— Masabaşı Oyun Kulübü Topluluğu</cite>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 4. Ne Tarz Oyunlar Oynanıyor? (4 Temel Kategori) -->
      <section class="section games-section" aria-labelledby="games-heading">
        <div class="container">
          <div class="section-header">
            <span class="section-eyebrow">Oyun Dünyamız</span>
            <h2 id="games-heading" class="section-title">Masalarımızda Hangi Oyunlar Oynanıyor?</h2>
            <p class="section-subtitle">
              Her zevke, her oyun tarzına ve her deneyim seviyesine uygun onlarca farklı oyun seçeneği sunuyoruz.
            </p>
          </div>

          <div class="games-grid">
            @for (cat of gameCategories; track cat.id) {
              <article class="game-category-card">
                <div class="game-category-card__header">
                  <span class="game-category-card__icon" aria-hidden="true">{{ cat.icon }}</span>
                  <span class="game-category-card__badge">{{ cat.badge }}</span>
                </div>

                <h3 class="game-category-card__title">{{ cat.title }}</h3>
                <p class="game-category-card__desc">{{ cat.description }}</p>

                <div class="game-category-card__chips">
                  @for (g of cat.games; track g) {
                    <span class="game-chip">{{ g }}</span>
                  }
                </div>

                <div class="game-category-card__footer">
                  <a [routerLink]="cat.detailRoute" class="card-link">
                    <span>{{ cat.detailLinkText }}</span>
                    <span aria-hidden="true">→</span>
                  </a>
                </div>
              </article>
            }
          </div>
        </div>
      </section>

      <!-- 5. Etkinliklere Nasıl Katılabilirsiniz? (4 Adımlı Rehber) -->
      <section class="section steps-section" aria-labelledby="steps-heading">
        <div class="container">
          <div class="section-header">
            <span class="section-eyebrow">İlk Kez Gelecekler İçin</span>
            <h2 id="steps-heading" class="section-title">Etkinliklerimize Nasıl Katılabilirsiniz?</h2>
            <p class="section-subtitle">
              Katılmak sandığınızdan çok daha kolay. Hiçbir sınav, ön hazırlık veya grup gereksinimi yok!
            </p>
          </div>

          <div class="steps-grid">
            @for (step of steps; track step.number) {
              <div class="step-card">
                <div class="step-card__number-badge">
                  <span>{{ step.number }}</span>
                </div>
                <h3 class="step-card__title">{{ step.title }}</h3>
                <p class="step-card__description">{{ step.description }}</p>
                <div class="step-card__highlight">
                  <span class="highlight-icon" aria-hidden="true">💡</span>
                  <span>{{ step.highlight }}</span>
                </div>
              </div>
            }
          </div>

          <div class="steps-cta">
            <div class="steps-cta__inner">
              <h3>Hemen Bir Sonraki Masada Yerinizi Alın!</h3>
              <p>Topluluk yöneticilerimiz ve diğer oyuncular sizinle tanışmak için sabırsızlanıyor.</p>
              <div class="steps-cta__buttons">
                <a [href]="meetupUrl" target="_blank" rel="noopener noreferrer" class="btn btn--primary">
                  Meetup Sayfamıza Git
                </a>
                <a [href]="instagramUrl" target="_blank" rel="noopener noreferrer" class="btn btn--secondary">
                  Instagram'dan Bize Yazın
                </a>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 6. Sıkça Sorulan Sorular (SSS) -->
      <section class="section faq-section" aria-labelledby="faq-heading">
        <div class="container">
          <div class="section-header">
            <span class="section-eyebrow">Merak Edilenler</span>
            <h2 id="faq-heading" class="section-title">Sıkça Sorulan Sorular</h2>
            <p class="section-subtitle">
              Aklınıza takılan soruların yanıtlarını aşağıda bulabilirsiniz.
            </p>
          </div>

          <div class="faq-list">
            @for (faq of faqs; track faq.id) {
              <article class="faq-item" [id]="faq.id">
                <h3 class="faq-item__question">
                  <span class="faq-icon" aria-hidden="true">❓</span>
                  <span>{{ faq.question }}</span>
                </h3>
                <p class="faq-item__answer">{{ faq.answer }}</p>
              </article>
            }
          </div>
        </div>
      </section>

      <!-- 7. Büyük Kapanış & Topluluk Çağrısı -->
      <section class="section cta-section" aria-labelledby="cta-heading">
        <div class="container">
          <div class="cta-banner">
            <span class="section-eyebrow">Kadıköy Buluşmaları</span>
            <h2 id="cta-heading" class="cta-banner__title">
              Zarlar Masada, Sıradaki Hamle Sizin!
            </h2>
            <p class="cta-banner__lead">
              Cumartesi gününüzü evde ekrana bakarak geçirmek yerine; sıcacık çaylar eşliğinde
              yeni insanlarla tanışın, zarlar atın ve unutulmaz masaüstü maceralarına ortak olun.
            </p>
            <div class="cta-banner__actions">
              <a [href]="meetupUrl" target="_blank" rel="noopener noreferrer" class="btn btn--primary">
                Meetup ile Üye Ol
              </a>
              <a [href]="instagramUrl" target="_blank" rel="noopener noreferrer" class="btn btn--secondary">
                Instagram Hesabımız (&#64;masabasikulubu)
              </a>
              <a [href]="facebookUrl" target="_blank" rel="noopener noreferrer" class="btn btn--ghost">
                Facebook Topluluğu
              </a>
            </div>
          </div>
        </div>
      </section>
    </main>
  `,
  styles: [`
    .home-page {
      padding-bottom: 5rem;
    }

    /* Hero Section */
    .home-hero {
      position: relative;
      padding: 4.5rem 0 3.5rem;
      background: radial-gradient(circle at 50% 20%, rgba(212, 175, 55, 0.08) 0%, transparent 60%);
      border-bottom: 1px solid var(--border-secondary-subtle);
    }

    .home-hero__container {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      max-width: 980px;
    }

    .home-hero__badge {
      display: inline-flex;
      align-items: center;
      gap: 0.6rem;
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      color: var(--accent-gold-primary);
      padding: 0.45rem 1.1rem;
      border-radius: 9999px;
      font-size: 0.85rem;
      font-weight: 700;
      letter-spacing: 0.04em;
      margin-bottom: 1.75rem;
    }

    .home-hero__badge-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background-color: var(--accent-success-seat);
      box-shadow: 0 0 8px var(--accent-success-seat);
    }

    .home-hero__title {
      font-size: 2.75rem;
      line-height: 1.15;
      color: var(--text-primary);
      letter-spacing: 0.05em;
      margin-bottom: 1.25rem;

      @media (min-width: 768px) {
        font-size: 4rem;
      }
    }

    .home-hero__lead {
      font-size: 1.25rem;
      line-height: 1.6;
      color: var(--accent-gold-primary);
      font-family: var(--font-display);
      max-width: 780px;
      margin-bottom: 1rem;

      @media (min-width: 768px) {
        font-size: 1.45rem;
      }
    }

    .home-hero__sublead {
      font-size: 1.05rem;
      line-height: 1.7;
      color: var(--text-muted);
      max-width: 720px;
      margin-bottom: 2.5rem;
    }

    .home-hero__actions {
      display: flex;
      flex-wrap: wrap;
      justify-content: center;
      gap: 1rem;
      margin-bottom: 3.5rem;
    }

    /* Buttons */
    .btn {
      display: inline-flex;
      align-items: center;
      gap: 0.6rem;
      font-family: var(--font-sans);
      font-size: 0.95rem;
      font-weight: 700;
      padding: 0.8rem 1.6rem;
      border-radius: 10px;
      text-decoration: none;
      transition: all 0.25s ease;
      cursor: pointer;
    }

    .btn--primary {
      background: linear-gradient(135deg, var(--accent-gold-primary) 0%, #b8860b 100%);
      color: #0c0e12;
      border: 1px solid var(--accent-gold-primary);
      box-shadow: 0 4px 14px rgba(212, 175, 55, 0.25);

      &:hover {
        transform: translateY(-2px);
        box-shadow: 0 6px 20px rgba(212, 175, 55, 0.4);
      }
    }

    .btn--secondary {
      background: var(--surface-secondary-walnut);
      color: var(--text-primary);
      border: 1px solid var(--border-secondary-subtle);

      &:hover {
        border-color: var(--accent-gold-primary);
        color: var(--accent-gold-primary);
        transform: translateY(-2px);
      }
    }

    .btn--ghost {
      background: transparent;
      color: var(--accent-gold-primary);
      border: 1px solid rgba(212, 175, 55, 0.4);

      &:hover {
        background: rgba(212, 175, 55, 0.1);
        border-color: var(--accent-gold-primary);
        transform: translateY(-2px);
      }
    }

    /* Hero Stats Grid */
    .home-hero__stats {
      display: grid;
      grid-template-columns: 1fr;
      gap: 1.25rem;
      width: 100%;
      text-align: left;

      @media (min-width: 640px) {
        grid-template-columns: repeat(2, 1fr);
      }

      @media (min-width: 1024px) {
        grid-template-columns: repeat(4, 1fr);
      }
    }

    .stat-card {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 12px;
      padding: 1.2rem;
      transition: all 0.25s ease;

      &:hover {
        border-color: rgba(212, 175, 55, 0.4);
        transform: translateY(-3px);
        box-shadow: 0 6px 16px rgba(0, 0, 0, 0.25);
      }
    }

    .stat-card__icon {
      font-size: 1.75rem;
      line-height: 1;
      padding-top: 0.1rem;
    }

    .stat-card__body strong {
      display: block;
      font-size: 0.95rem;
      color: var(--accent-gold-primary);
      margin-bottom: 0.25rem;
    }

    .stat-card__body span {
      font-size: 0.85rem;
      color: var(--text-muted);
      line-height: 1.4;
    }

    /* General Section Styles */
    .section {
      padding: 4.5rem 0;
    }

    .section-header {
      text-align: center;
      max-width: 760px;
      margin: 0 auto 3rem;
    }

    .section-eyebrow {
      display: inline-block;
      font-family: var(--font-sans);
      font-size: 0.8rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.15em;
      color: var(--accent-gold-primary);
      margin-bottom: 0.6rem;
    }

    .section-title {
      font-size: 2.15rem;
      color: var(--text-primary);
      line-height: 1.25;
      margin-bottom: 1rem;

      @media (min-width: 768px) {
        font-size: 2.65rem;
      }
    }

    .section-subtitle {
      font-size: 1.05rem;
      line-height: 1.7;
      color: var(--text-muted);
      margin: 0;
    }

    /* About Section */
    .about-section {
      background: var(--bg-dominant-elevated);
      border-bottom: 1px solid var(--border-secondary-subtle);
    }

    .about-grid {
      display: grid;
      grid-template-columns: 1fr;
      gap: 1.75rem;

      @media (min-width: 768px) {
        grid-template-columns: repeat(3, 1fr);
      }
    }

    .about-card {
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 14px;
      padding: 2rem 1.75rem;
      transition: all 0.25s ease;

      &:hover {
        border-color: var(--accent-gold-primary);
        box-shadow: 0 8px 24px rgba(0, 0, 0, 0.35);
        transform: translateY(-4px);
      }
    }

    .about-card__icon-wrapper {
      width: 52px;
      height: 52px;
      border-radius: 12px;
      background: rgba(212, 175, 55, 0.12);
      border: 1px solid rgba(212, 175, 55, 0.3);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.35rem;
    }

    .about-card__icon {
      font-size: 1.6rem;
    }

    .about-card__title {
      font-size: 1.3rem;
      color: var(--text-primary);
      margin-bottom: 0.85rem;
    }

    .about-card__text {
      font-size: 0.95rem;
      line-height: 1.65;
      color: var(--text-muted);
      margin: 0;
    }

    /* Venue Section */
    .venue-section {
      border-bottom: 1px solid var(--border-secondary-subtle);
    }

    .venue-card {
      display: grid;
      grid-template-columns: 1fr;
      gap: 2.5rem;
      background: linear-gradient(145deg, var(--surface-secondary-walnut) 0%, #151922 100%);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 18px;
      padding: 2.5rem 2rem;

      @media (min-width: 900px) {
        grid-template-columns: 3fr 2fr;
        padding: 3.5rem 3rem;
        align-items: center;
      }
    }

    .venue-card__title {
      font-size: 2.15rem;
      color: var(--text-primary);
      margin-bottom: 0.75rem;
    }

    .venue-card__lead {
      font-size: 1.1rem;
      color: var(--accent-gold-primary);
      line-height: 1.6;
      margin-bottom: 2rem;
    }

    .venue-details {
      display: flex;
      flex-direction: column;
      gap: 1.35rem;
      margin-bottom: 2rem;
    }

    .venue-item {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
    }

    .venue-item__icon {
      font-size: 1.5rem;
      line-height: 1;
      padding-top: 0.15rem;
    }

    .venue-item strong {
      display: block;
      font-size: 0.95rem;
      color: var(--text-primary);
      margin-bottom: 0.2rem;
    }

    .venue-item p {
      font-size: 0.9rem;
      color: var(--text-muted);
      line-height: 1.5;
      margin: 0;
    }

    .venue-card__side {
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .quote-box {
      background: rgba(16, 22, 28, 0.75);
      border: 1px solid rgba(212, 175, 55, 0.35);
      border-radius: 14px;
      padding: 2.25rem 2rem;
      text-align: center;
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
    }

    .quote-box__emblem {
      font-size: 2.5rem;
      margin-bottom: 1rem;
    }

    .quote-box__quote {
      font-family: var(--font-display);
      font-style: italic;
      font-size: 1.15rem;
      line-height: 1.65;
      color: var(--text-primary);
      margin: 0 0 1.25rem;
    }

    .quote-box__author {
      font-size: 0.85rem;
      color: var(--accent-gold-primary);
      font-weight: 700;
      letter-spacing: 0.05em;
    }

    /* Games Section */
    .games-section {
      background: var(--bg-dominant-elevated);
      border-bottom: 1px solid var(--border-secondary-subtle);
    }

    .games-grid {
      display: grid;
      grid-template-columns: 1fr;
      gap: 1.75rem;

      @media (min-width: 640px) {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    .game-category-card {
      display: flex;
      flex-direction: column;
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 14px;
      padding: 2rem;
      transition: all 0.25s ease;

      &:hover {
        border-color: var(--accent-gold-primary);
        box-shadow: 0 8px 24px rgba(0, 0, 0, 0.35);
        transform: translateY(-4px);
      }
    }

    .game-category-card__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 1.25rem;
    }

    .game-category-card__icon {
      font-size: 2.2rem;
    }

    .game-category-card__badge {
      font-size: 0.75rem;
      font-weight: 700;
      color: var(--accent-gold-primary);
      background: rgba(212, 175, 55, 0.12);
      border: 1px solid rgba(212, 175, 55, 0.3);
      padding: 0.3rem 0.75rem;
      border-radius: 9999px;
    }

    .game-category-card__title {
      font-size: 1.45rem;
      color: var(--text-primary);
      margin-bottom: 0.75rem;
    }

    .game-category-card__desc {
      font-size: 0.95rem;
      color: var(--text-muted);
      line-height: 1.6;
      margin-bottom: 1.25rem;
      flex-grow: 1;
    }

    .game-category-card__chips {
      display: flex;
      flex-wrap: wrap;
      gap: 0.45rem;
      margin-bottom: 1.5rem;
    }

    .game-chip {
      background: rgba(255, 255, 255, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.08);
      font-size: 0.8rem;
      font-weight: 600;
      color: var(--text-primary);
      padding: 0.25rem 0.65rem;
      border-radius: 6px;
    }

    .card-link {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      color: var(--accent-gold-primary);
      font-weight: 700;
      font-size: 0.9rem;
      text-decoration: none;
      transition: gap 0.2s ease;

      &:hover {
        gap: 0.8rem;
        text-decoration: underline;
      }
    }

    /* Steps Section */
    .steps-section {
      border-bottom: 1px solid var(--border-secondary-subtle);
    }

    .steps-grid {
      display: grid;
      grid-template-columns: 1fr;
      gap: 1.5rem;
      margin-bottom: 3.5rem;

      @media (min-width: 640px) {
        grid-template-columns: repeat(2, 1fr);
      }

      @media (min-width: 1024px) {
        grid-template-columns: repeat(4, 1fr);
      }
    }

    .step-card {
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 14px;
      padding: 1.75rem 1.5rem;
      display: flex;
      flex-direction: column;
      position: relative;
      transition: all 0.25s ease;

      &:hover {
        border-color: var(--accent-gold-primary);
        transform: translateY(-4px);
      }
    }

    .step-card__number-badge {
      width: 44px;
      height: 44px;
      border-radius: 10px;
      background: linear-gradient(135deg, var(--surface-secondary-emerald) 0%, var(--surface-secondary-walnut) 100%);
      border: 1.5px solid var(--accent-gold-primary);
      display: flex;
      align-items: center;
      justify-content: center;
      font-family: var(--font-display);
      font-size: 1.25rem;
      font-weight: 800;
      color: var(--accent-gold-primary);
      margin-bottom: 1.25rem;
    }

    .step-card__title {
      font-size: 1.15rem;
      color: var(--text-primary);
      margin-bottom: 0.75rem;
    }

    .step-card__description {
      font-size: 0.9rem;
      line-height: 1.6;
      color: var(--text-muted);
      margin-bottom: 1.25rem;
      flex-grow: 1;
    }

    .step-card__highlight {
      display: flex;
      align-items: flex-start;
      gap: 0.5rem;
      background: rgba(212, 175, 55, 0.08);
      border-left: 3px solid var(--accent-gold-primary);
      padding: 0.6rem 0.75rem;
      border-radius: 0 6px 6px 0;
      font-size: 0.8rem;
      color: var(--text-primary);
      line-height: 1.4;
    }

    .highlight-icon {
      font-size: 0.9rem;
      line-height: 1;
    }

    .steps-cta {
      background: linear-gradient(135deg, rgba(212, 175, 55, 0.1) 0%, rgba(16, 185, 129, 0.05) 100%);
      border: 1px solid rgba(212, 175, 55, 0.3);
      border-radius: 16px;
      padding: 2.5rem;
      text-align: center;
    }

    .steps-cta__inner h3 {
      font-size: 1.75rem;
      color: var(--text-primary);
      margin-bottom: 0.5rem;
    }

    .steps-cta__inner p {
      color: var(--text-muted);
      font-size: 1rem;
      margin-bottom: 1.75rem;
    }

    .steps-cta__buttons {
      display: flex;
      flex-wrap: wrap;
      justify-content: center;
      gap: 1rem;
    }

    /* FAQ Section */
    .faq-section {
      background: var(--bg-dominant-elevated);
      border-bottom: 1px solid var(--border-secondary-subtle);
    }

    .faq-list {
      max-width: 820px;
      margin: 0 auto;
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }

    .faq-item {
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 12px;
      padding: 1.75rem 2rem;
      transition: all 0.2s ease;

      &:hover {
        border-color: rgba(212, 175, 55, 0.4);
      }
    }

    .faq-item__question {
      display: flex;
      align-items: flex-start;
      gap: 0.75rem;
      font-size: 1.15rem;
      color: var(--accent-gold-primary);
      margin-bottom: 0.75rem;
      line-height: 1.35;
    }

    .faq-icon {
      font-size: 1.2rem;
      line-height: 1;
    }

    .faq-item__answer {
      font-size: 0.95rem;
      line-height: 1.65;
      color: var(--text-muted);
      margin: 0;
      padding-left: 1.95rem;
    }

    /* Final CTA Banner */
    .cta-section {
      padding: 4.5rem 0 2rem;
    }

    .cta-banner {
      background: radial-gradient(circle at 50% 50%, rgba(212, 175, 55, 0.15) 0%, #151922 80%);
      border: 1px solid var(--accent-gold-primary);
      border-radius: 20px;
      padding: 3.5rem 2rem;
      text-align: center;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.4);

      @media (min-width: 768px) {
        padding: 4.5rem 3.5rem;
      }
    }

    .cta-banner__title {
      font-size: 2.35rem;
      color: var(--text-primary);
      margin-bottom: 1.25rem;

      @media (min-width: 768px) {
        font-size: 3.1rem;
      }
    }

    .cta-banner__lead {
      font-size: 1.15rem;
      line-height: 1.7;
      color: var(--text-muted);
      max-width: 720px;
      margin: 0 auto 2.5rem;
    }

    .cta-banner__actions {
      display: flex;
      flex-wrap: wrap;
      justify-content: center;
      gap: 1rem;
    }
  `]
})
export class HomePageComponent implements OnInit {
  private readonly seoMetaService = inject(SeoMetaService);
  private readonly jsonLdSchemaService = inject(JsonLdSchemaService);

  public readonly meetupUrl = environment.communityUrls.meetup;
  public readonly instagramUrl = environment.communityUrls.instagram;
  public readonly facebookUrl = environment.communityUrls.facebook;

  public readonly gameCategories: GameCategoryShowcase[] = [
    {
      id: 'board-games',
      icon: '♟️',
      badge: 'Strateji & Kaynak',
      title: 'Modern Kutu Oyunları',
      games: ['Catan', 'Ticket to Ride: Europe', 'Carcassonne', 'Terraforming Mars', '7 Wonders', 'Wingspan'],
      description: 'Zar atmanın ötesine geçen, kaynak yönetimi, takas, rota kurma ve derin strateji sunan dünyanın en seçkin masaüstü oyunları.',
      detailLinkText: 'Kutu Oyunu Kütüphanesini İncele',
      detailRoute: '/oyun-kutuphanesi'
    },
    {
      id: 'frp-dnd',
      icon: '⚔️',
      badge: 'Rol Yapma & Hikaye',
      title: 'D&D 5e ve Masaüstü FRP',
      games: ['Dungeons & Dragons 5e', 'Call of Cthulhu', 'One-Shot Maceralar', 'Acemi Masaları'],
      description: 'Dungeon Master (DM) eşliğinde kendi kahramanınızı yaratıp zarların ve hayal gücünüzün rehberliğinde fantastik zindanlara adım atın.',
      detailLinkText: 'D&D 5e Başlangıç Rehberini Oku',
      detailRoute: '/frp-dnd-rehberi'
    },
    {
      id: 'social-deduction',
      icon: '🎭',
      badge: 'Blöf & Kahkaha',
      title: 'Sosyal Çıkarım & Parti',
      games: ['Secret Hitler', 'The Resistance: Avalon', 'Blood on the Clocktower', 'Codenames', 'Spyfall'],
      description: 'Kim faşist, kim büyücü, kim doğruyu söylüyor? Kalabalık gruplarla buzları eriten, bol kahkahalı ve dedüksiyon dolu masa oyunları.',
      detailLinkText: 'Sosyal Çıkarım Stratejileri',
      detailRoute: '/blog'
    },
    {
      id: 'quick-fun',
      icon: '🃏',
      badge: 'Hızlı & Dinamik',
      title: 'Hafif & Eğlenceli Kart Oyunları',
      games: ['Love Letter', 'Exploding Kittens', 'Saboteur', 'Dixit', 'Sushi Go!'],
      description: '5 dakikada öğrenilen, hızlı turlarla dönen, çayınızı içerken neşeyle masayı ısıtan hafif ve eğlenceli parti oyunları.',
      detailLinkText: 'Kütüphaneyi Keşfet',
      detailRoute: '/oyun-kutuphanesi'
    }
  ];

  public readonly steps: StepItem[] = [
    {
      number: '01',
      title: 'Topluluğa Katılın',
      description: 'Meetup sayfamıza üye olun veya Instagram hesabımızı (@masabasikulubu) takip edin. Haftalık Cumartesi duyuruları ilk olarak buralarda paylaşılır.',
      highlight: 'Üyelik tamamen ücretsizdir.'
    },
    {
      number: '02',
      title: 'Tek Başınıza Gelin',
      description: 'Katılmak için önceden bir oyun ekibine sahip olmanız gerekmez! Topluluğumuzdaki insanların çoğu ilk etkinliğe tek başına gelerek yeni arkadaşlar edinmiştir.',
      highlight: 'Kapıda karşılanıp açık masalara yönlendirilirsiniz.'
    },
    {
      number: '03',
      title: 'Kurallar Masada Öğretilir',
      description: 'Daha önce hayatınızda hiç kutu oyunu oynamamış olsanız bile sorun değil. Her masada oyun kurallarını sıfırdan anlatan tecrübeli moderatörlerimiz vardır.',
      highlight: 'Soru sormaktan ve hata yapmaktan asla çekinmeyin.'
    },
    {
      number: '04',
      title: 'Masaya Oturun & Eğlenin',
      description: 'Çayınızı, kahvenizi alın, zarlarınızı atın ve saatlerce süren keyifli masaüstü maceralarının ve yeni dostlukların tadını çıkarın.',
      highlight: 'İstediğiniz saatte gelip ayrılabilirsiniz.'
    }
  ];

  public readonly faqs: FaqItem[] = [
    {
      id: 'faq-1',
      question: 'Daha önce hiç modern kutu oyunu veya FRP oynamadım, gelebilir miyim?',
      answer: 'Kesinlikle evet! Masabaşı Oyun Kulübü tam olarak bu duvarı yıkmak için kuruldu. Buluşmalarımızda her masada kuralları sabırla anlatan bir moderatör veya deneyimli oyuncu bulunur. İlk defa zar atacak olanlar baş tacımızdır.'
    },
    {
      id: 'faq-2',
      question: 'Tek başıma gelebilir miyim, yanımda arkadaş getirmem şart mı?',
      answer: 'Hayır, kesinlikle şart değil! Katılımcılarımızın yarısından fazlası ilk etkinliğe tek başına gelir. Kapıdan girdiğiniz anda sizi karşılar, masadaki boş koltuklara ve ilginizi çeken oyunlara dahil ederiz.'
    },
    {
      id: 'faq-3',
      question: 'Etkinlikler ücretli mi, nasıl bir işleyiş var?',
      answer: 'Masabaşı Oyun Kulübü kâr amacı gütmeyen bağımsız bir topluluktur. Kulüp için katılım ücreti talep edilmez. Yalnızca buluştuğumuz Kadıköy mekanının/kafesinin standart masa/giriş veya yeme-içme kuralları geçerlidir.'
    },
    {
      id: 'faq-4',
      question: 'Kendi kutu oyunumu getirebilir miyim?',
      answer: 'Elbette! Rafta oynanmayı bekleyen veya diğer üyelere öğretmek istediğiniz oyunlarınızı getirebilir, masanızda duyurarak diğer katılımcılarla birlikte oynayabilirsiniz.'
    },
    {
      id: 'faq-5',
      question: 'Buluşma nerede ve kaçta yapılıyor?',
      answer: 'Her Cumartesi günü saat 11:00 ile 18:00+ arasında İstanbul Kadıköy merkezindeki seçkin ve ferah mekanlarda toplanıyoruz. Güncel adres detayları haftalık olarak Meetup ve Instagram üzerinden paylaşılmaktadır.'
    }
  ];

  ngOnInit(): void {
    this.seoMetaService.setMeta({
      title: 'Masabaşı Oyun Kulübü | Kadıköy Modern Kutu Oyunları & FRP Topluluğu',
      description: 'İstanbul Kadıköy merkezli bağımsız masaüstü oyun kulübü. Her Cumartesi modern kutu oyunları (Catan, Avalon, Terraforming Mars) ve yeni başlayanlara açık D&D 5e FRP masaları.',
      canonicalUrl: 'https://masabasikulubu.com/'
    });

    this.jsonLdSchemaService.setSchema([
      this.jsonLdSchemaService.buildOrganizationSchema()
    ]);
  }
}
