import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <footer class="footer">
      <div class="container footer__container">
        <!-- Sol Bölüm: Kulüp Kimliği -->
        <div class="footer__brand-col">
          <div class="footer__logo">
            <span class="footer__icon" aria-hidden="true">🎲</span>
            <span class="footer__title">MASABAŞI OYUN KULÜBÜ</span>
          </div>
          <p class="footer__desc">
            İstanbul'da her Cumartesi modern kutu oyunları, sosyal çıkarım ve D&D 5e FRP maceraları için toplanan bağımsız masaüstü oyun topluluğu.
          </p>
          <div class="footer__socials">
            <a
              href="https://www.instagram.com/masabasikulubu/"
              target="_blank"
              rel="noopener noreferrer"
              class="footer__social-link"
              aria-label="Instagram">
              📸 Instagram
            </a>
            <a
              href="https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/events/316644892/"
              target="_blank"
              rel="noopener noreferrer"
              class="footer__social-link"
              aria-label="Meetup">
              🤝 Meetup
            </a>
            <a
              href="https://www.facebook.com/groups/14080585061/user/61595360127902/"
              target="_blank"
              rel="noopener noreferrer"
              class="footer__social-link"
              aria-label="Facebook">
              👥 Facebook
            </a>
          </div>
        </div>

        <!-- Orta Bölüm: Hızlı Bağlantılar -->
        <div class="footer__links-col">
          <h4 class="footer__heading">Keşfet</h4>
          <ul class="footer__links">
            <li><a routerLink="/">Ana Sayfa</a></li>
            <li><a routerLink="/etkinlikler">Buluşmalar & Masalar</a></li>
            <li><a routerLink="/oyun-kutuphanesi">Kutu Oyunu Kütüphanesi</a></li>
            <li><a routerLink="/frp-dnd-rehberi">D&D 5e Başlangıç Kılavuzu</a></li>
            <li><a routerLink="/blog">Rehberler ve Blog</a></li>
          </ul>
        </div>

        <!-- Sağ Bölüm: GEO, AI ve Standartlar -->
        <div class="footer__links-col">
          <h4 class="footer__heading">Yapay Zeka & Standartlar</h4>
          <ul class="footer__links">
            <li><a href="/llms.txt" target="_blank" rel="noopener">LLMs.txt (AI Dizini)</a></li>
            <li><a href="/sitemap.xml" target="_blank" rel="noopener">XML Site Haritası</a></li>
            <li><a routerLink="/admin" class="footer__dim">🛡️ Yönetim Paneli</a></li>
            <li><span class="footer__dim">Dual-Mode: API & Statik Dayanıklı</span></li>
          </ul>
        </div>
      </div>

      <div class="footer__bottom">
        <div class="container footer__bottom-inner">
          <p class="footer__copy">
            © 2026 Masabaşı Oyun Kulübü. Tüm hakları saklıdır.
          </p>
          <p class="footer__tagline">
            "Zarlar atılsın, masalar birleşsin."
          </p>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background-color: var(--bg-dominant-elevated);
      border-top: 1px solid var(--border-secondary-subtle);
      margin-top: 5rem;
      padding-top: 3.5rem;
      color: var(--text-muted);
    }

    .footer__container {
      display: grid;
      grid-template-columns: 1fr;
      gap: 2.5rem;

      @media (min-width: 768px) {
        grid-template-columns: 2fr 1fr 1fr;
      }
    }

    .footer__brand-col {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .footer__logo {
      display: flex;
      align-items: center;
      gap: 0.6rem;
    }

    .footer__icon {
      font-size: 1.5rem;
    }

    .footer__title {
      font-family: var(--font-display);
      font-weight: 800;
      color: var(--accent-gold-primary);
      letter-spacing: 0.08em;
      font-size: 1.15rem;
    }

    .footer__desc {
      font-size: 0.9rem;
      line-height: 1.6;
      max-width: 440px;
      margin: 0;
    }

    .footer__socials {
      display: flex;
      flex-wrap: wrap;
      gap: 0.75rem;
      margin-top: 0.5rem;
    }

    .footer__social-link {
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      color: var(--text-primary);
      font-size: 0.85rem;
      font-weight: 600;
      padding: 0.35rem 0.75rem;
      border-radius: 6px;
      text-decoration: none;
      transition: all 0.2s ease;

      &:hover {
        border-color: var(--accent-gold-primary);
        color: var(--accent-gold-primary);
        background: var(--surface-secondary-emerald);
      }
    }

    .footer__heading {
      font-size: 1rem;
      color: var(--text-primary);
      margin-bottom: 1rem;
    }

    .footer__links {
      list-style: none;
      display: flex;
      flex-direction: column;
      gap: 0.6rem;
    }

    .footer__links a {
      color: var(--text-muted);
      text-decoration: none;
      font-size: 0.9rem;
      transition: color 0.2s ease;

      &:hover {
        color: var(--accent-gold-primary);
      }
    }

    .footer__dim {
      font-size: 0.85rem;
      color: rgba(184, 176, 164, 0.5);
    }

    .footer__bottom {
      border-top: 1px solid rgba(47, 79, 70, 0.4);
      margin-top: 3rem;
      padding: 1.5rem 0;
      font-size: 0.85rem;
    }

    .footer__bottom-inner {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      text-align: center;

      @media (min-width: 600px) {
        flex-direction: row;
        justify-content: space-between;
      }
    }

    .footer__copy {
      margin: 0;
    }

    .footer__tagline {
      font-family: var(--font-display);
      font-style: italic;
      color: var(--accent-gold-primary);
      margin: 0;
    }
  `]
})
export class FooterComponent {}
