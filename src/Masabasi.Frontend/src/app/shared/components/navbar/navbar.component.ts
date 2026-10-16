import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ThemeService } from '../../../core/services/theme.service';
import { ActiveModeBadgeComponent } from '../active-mode-badge/active-mode-badge.component';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, ActiveModeBadgeComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <header class="navbar">
      <div class="container navbar__container">
        <!-- Logo ve Marka İsmi -->
        <a routerLink="/" class="navbar__brand" aria-label="Masabaşı Oyun Kulübü Ana Sayfa">
          <div class="navbar__emblem" aria-hidden="true">
            <span class="d20-icon">🎲</span>
          </div>
          <div class="navbar__brand-text">
            <span class="navbar__title">MASABAŞI</span>
            <span class="navbar__subtitle">OYUN KULÜBÜ</span>
          </div>
        </a>

        <!-- Masaüstü Navigasyon Menüsü -->
        <nav class="navbar__nav" aria-label="Ana Menü">
          <a routerLink="/etkinlikler" routerLinkActive="active" class="navbar__link">
            Cumartesi Masaları
          </a>
          <a routerLink="/oyun-kutuphanesi" routerLinkActive="active" class="navbar__link">
            Oyun Kütüphanesi
          </a>
          <a routerLink="/frp-dnd-rehberi" routerLinkActive="active" class="navbar__link">
            FRP & D&D Rehberi
          </a>
          <a routerLink="/blog" routerLinkActive="active" class="navbar__link">
            Rehberler & Blog
          </a>
        </nav>

        <!-- Sağ Taraf: Canlı/Statik Rozet, Tema Butonu ve Mobil Menü Butonu -->
        <div class="navbar__actions">
          <app-active-mode-badge class="navbar__mode-badge" />

          <!-- Tema Değiştirici -->
          <button
            type="button"
            class="navbar__theme-btn"
            (click)="themeService.toggleTheme()"
            [attr.aria-label]="themeService.currentTheme() === 'taverna' ? 'Parşömen açık temaya geç' : 'Taverna koyu temaya geç'"
            [attr.title]="themeService.currentTheme() === 'taverna' ? 'Parşömen Temasına Geç' : 'Taverna Koyu Temasına Geç'">
            {{ themeService.currentTheme() === 'taverna' ? '📜 Parşömen' : '🌙 Taverna' }}
          </button>

          <!-- Mobil Menü Butonu -->
          <button
            type="button"
            class="navbar__mobile-toggle"
            (click)="toggleMobileMenu()"
            [attr.aria-expanded]="isMobileMenuOpen()"
            aria-label="Menüyü Aç/Kapat">
            <span class="hamburger-bar"></span>
            <span class="hamburger-bar"></span>
            <span class="hamburger-bar"></span>
          </button>
        </div>
      </div>

      <!-- Mobil Açılır Menü -->
      @if (isMobileMenuOpen()) {
        <nav class="navbar__mobile-nav" aria-label="Mobil Menü">
          <a routerLink="/etkinlikler" (click)="closeMobileMenu()" class="navbar__mobile-link">
            Cumartesi Masaları
          </a>
          <a routerLink="/oyun-kutuphanesi" (click)="closeMobileMenu()" class="navbar__mobile-link">
            Oyun Kütüphanesi
          </a>
          <a routerLink="/frp-dnd-rehberi" (click)="closeMobileMenu()" class="navbar__mobile-link">
            FRP & D&D Rehberi
          </a>
          <a routerLink="/blog" (click)="closeMobileMenu()" class="navbar__mobile-link">
            Rehberler & Blog
          </a>
        </nav>
      }
    </header>
  `,
  styles: [`
    .navbar {
      position: sticky;
      top: 0;
      z-index: 100;
      background: var(--bg-glass);
      backdrop-filter: blur(14px);
      -webkit-backdrop-filter: blur(14px);
      border-bottom: 1px solid var(--border-secondary-subtle);
      transition: background 0.3s ease;
    }

    .navbar__container {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 72px;
    }

    .navbar__brand {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      text-decoration: none;
    }

    .navbar__emblem {
      width: 42px;
      height: 42px;
      border-radius: 10px;
      background: linear-gradient(135deg, var(--surface-secondary-emerald) 0%, var(--surface-secondary-walnut) 100%);
      border: 1.5px solid var(--accent-gold-primary);
      display: flex;
      align-items: center;
      justify-content: center;
      box-shadow: 0 0 10px rgba(212, 175, 55, 0.25);
    }

    .d20-icon {
      font-size: 1.3rem;
    }

    .navbar__brand-text {
      display: flex;
      flex-direction: column;
    }

    .navbar__title {
      font-family: var(--font-display);
      font-size: 1.2rem;
      font-weight: 800;
      color: var(--accent-gold-primary);
      letter-spacing: 0.1em;
      line-height: 1;
    }

    .navbar__subtitle {
      font-family: var(--font-sans);
      font-size: 0.65rem;
      font-weight: 700;
      color: var(--text-muted);
      letter-spacing: 0.18em;
      margin-top: 0.2rem;
    }

    .navbar__nav {
      display: none;
      align-items: center;
      gap: 1.5rem;

      @media (min-width: 900px) {
        display: flex;
      }
    }

    .navbar__link {
      font-family: var(--font-sans);
      font-weight: 600;
      font-size: 0.95rem;
      color: var(--text-muted);
      text-decoration: none;
      position: relative;
      padding: 0.5rem 0;
      transition: color 0.2s ease;

      &:hover, &.active {
        color: var(--accent-gold-primary);
      }

      &.active::after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 0;
        width: 100%;
        height: 2px;
        background-color: var(--accent-gold-primary);
        border-radius: 2px;
      }
    }

    .navbar__actions {
      display: flex;
      align-items: center;
      gap: 0.85rem;
    }

    .navbar__mode-badge {
      display: none;
      @media (min-width: 600px) {
        display: block;
      }
    }

    .navbar__theme-btn {
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      color: var(--accent-gold-primary);
      font-family: var(--font-sans);
      font-size: 0.8rem;
      font-weight: 700;
      padding: 0.4rem 0.8rem;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        border-color: var(--accent-gold-primary);
        background: var(--surface-secondary-emerald);
      }
    }

    .navbar__mobile-toggle {
      display: flex;
      flex-direction: column;
      justify-content: space-around;
      width: 32px;
      height: 32px;
      background: transparent;
      border: none;
      cursor: pointer;
      padding: 4px;

      @media (min-width: 900px) {
        display: none;
      }
    }

    .hamburger-bar {
      width: 100%;
      height: 2.5px;
      background-color: var(--accent-gold-primary);
      border-radius: 2px;
    }

    .navbar__mobile-nav {
      display: flex;
      flex-direction: column;
      background: var(--bg-dominant-elevated);
      border-top: 1px solid var(--border-secondary-subtle);
      border-bottom: 1px solid var(--border-secondary-subtle);
      padding: 1rem 1.5rem;
      gap: 0.75rem;

      @media (min-width: 900px) {
        display: none;
      }
    }

    .navbar__mobile-link {
      font-weight: 600;
      font-size: 1.05rem;
      color: var(--text-primary);
      text-decoration: none;
      padding: 0.5rem 0;

      &:hover {
        color: var(--accent-gold-primary);
      }
    }
  `]
})
export class NavbarComponent {
  public readonly themeService = inject(ThemeService);
  public readonly isMobileMenuOpen = signal<boolean>(false);

  public toggleMobileMenu(): void {
    this.isMobileMenuOpen.update((open) => !open);
  }

  public closeMobileMenu(): void {
    this.isMobileMenuOpen.set(false);
  }
}
