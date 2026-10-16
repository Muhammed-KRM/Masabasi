import { Component, ChangeDetectionStrategy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';
import { BoardGameModel } from '../../core/models/board-game.model';
import { TldrAiBoxComponent } from '../../shared/components/tldr-ai-box/tldr-ai-box.component';

@Component({
  selector: 'app-game-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TldrAiBoxComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="game-detail-page container">
      @if (isLoading()) {
        <div class="loading-state">
          <div class="spinner"></div>
          <p>Oyun kuralları ve detayları yükleniyor...</p>
        </div>
      } @else if (game(); as g) {
        <nav class="breadcrumb" aria-label="Gezinme Yolu">
          <a routerLink="/oyun-kutuphanesi">← Oyun Kütüphanesine Dön</a>
        </nav>

        <article class="game-article">
          <!-- Üst Başlık Bölümü -->
          <header class="game-header">
            <div class="game-header__badges">
              <span class="badge badge--emerald">{{ getCategoryLabel(g.category) }}</span>
              @if (g.yearPublished) {
                <span class="badge badge--dark">📅 {{ g.yearPublished }}</span>
              }
              @if (g.isInClubInventory) {
                <span class="badge badge--inv">✓ Kulüp Envanterinde Mevcut</span>
              }
            </div>

            <h1 class="game-header__title">{{ g.title }}</h1>

            <!-- TL;DR AI & GEO Özet Kutusu -->
            <app-tldr-ai-box [summary]="g.tldrSummary" />

            <!-- Oyun Metrikleri Paneli -->
            <div class="game-metrics">
              <div class="metric-card">
                <span class="metric-card__icon" aria-hidden="true">👥</span>
                <div class="metric-card__info">
                  <span class="label">Oyuncu Sayısı</span>
                  <span class="value">{{ g.minPlayers }} - {{ g.maxPlayers }} Kişi</span>
                  @if (g.recommendedPlayerCount) {
                    <small class="subtext">İdeal: {{ g.recommendedPlayerCount }}</small>
                  }
                </div>
              </div>

              <div class="metric-card">
                <span class="metric-card__icon" aria-hidden="true">⏱️</span>
                <div class="metric-card__info">
                  <span class="label">Ortalama Süre</span>
                  <span class="value">{{ g.playTimeMinutes }} Dakika</span>
                </div>
              </div>

              <div class="metric-card">
                <span class="metric-card__icon" aria-hidden="true">🧠</span>
                <div class="metric-card__info">
                  <span class="label">Oyun Karmaşıklığı</span>
                  <span class="value">{{ g.complexityRating }} / 5.0</span>
                </div>
              </div>
            </div>
          </header>

          <!-- Kurallar ve Oynanış Rehberi -->
          <section class="game-content" aria-labelledby="rules-heading">
            <h2 id="rules-heading" class="section-title">Oynanış ve Kural Kılavuzu</h2>
            <div class="rules-markdown-box">
              <p class="rules-lead">
                {{ g.title }} oynamak için tüm kuralları ezberlemenize gerek yok. Masabaşı Oyun Kulübü
                masalarında oyun moderatörlerimiz ve DM'lerimiz ilk turdan itibaren kuralları uygulamalı
                olarak anlatmaktadır.
              </p>
              <pre class="rules-pre">{{ g.rulesMarkdown }}</pre>
            </div>
          </section>

          <!-- CTA: Masaya Katıl -->
          <section class="play-cta-box">
            <div class="play-cta-box__content">
              <h3>{{ g.title }} Masasında Yerini Al!</h3>
              <p>
                Her Cumartesi İstanbul'da düzenlediğimiz buluşmalarda bu oyunu diğer kulüp üyeleriyle
                birlikte oynayabilirsiniz.
              </p>
            </div>
            <a routerLink="/etkinlikler" class="btn-cta-gold">
              Cumartesi Masalarını İncele →
            </a>
          </section>
        </article>
      } @else {
        <div class="not-found-state">
          <h2>Aradığınız Oyun Bulunamadı</h2>
          <p>Kütüphanemizde bu ada sahip bir oyun veya kural sayfası bulunmuyor.</p>
          <a routerLink="/oyun-kutuphanesi" class="btn-cta-gold">
            Kütüphaneye Göz At
          </a>
        </div>
      }
    </main>
  `,
  styles: [`
    .game-detail-page {
      padding: 2rem 1rem 4rem;
      max-width: 900px;
      margin: 0 auto;
    }
    .breadcrumb a {
      color: var(--accent-gold-primary);
      text-decoration: none;
      font-weight: 600;
      font-size: 0.95rem;
    }
    .breadcrumb a:hover {
      text-decoration: underline;
    }
    .game-article {
      margin-top: 1.5rem;
    }
    .game-header {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }
    .game-header__badges {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }
    .badge {
      font-size: 0.8rem;
      font-weight: 700;
      padding: 0.25rem 0.65rem;
      border-radius: 6px;
    }
    .badge--emerald {
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
    }
    .badge--dark {
      background-color: var(--bg-dominant-elevated);
      color: var(--text-muted);
    }
    .badge--inv {
      background-color: rgba(52, 211, 153, 0.15);
      color: var(--accent-success-seat);
      border: 1px solid var(--accent-success-seat);
    }
    .game-header__title {
      font-family: var(--font-display);
      font-size: clamp(2.2rem, 5vw, 3.2rem);
      color: var(--text-primary);
      margin: 0;
      line-height: 1.2;
    }
    .game-metrics {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
      margin-top: 0.5rem;
    }
    .metric-card {
      background-color: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 10px;
      padding: 1rem;
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    .metric-card__icon {
      font-size: 2rem;
    }
    .metric-card__info {
      display: flex;
      flex-direction: column;
    }
    .metric-card__info .label {
      font-size: 0.8rem;
      color: var(--text-muted);
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .metric-card__info .value {
      font-size: 1.15rem;
      font-weight: 700;
      color: var(--text-primary);
    }
    .metric-card__info .subtext {
      font-size: 0.75rem;
      color: var(--accent-gold-primary);
    }
    .game-content {
      margin-top: 3rem;
    }
    .section-title {
      font-family: var(--font-display);
      font-size: 1.75rem;
      color: var(--accent-gold-primary);
      margin-bottom: 1rem;
      border-bottom: 1px solid var(--border-secondary-subtle);
      padding-bottom: 0.5rem;
    }
    .rules-lead {
      color: var(--text-primary);
      font-size: 1.05rem;
      line-height: 1.6;
      margin-bottom: 1.5rem;
    }
    .rules-pre {
      background-color: var(--bg-dominant-elevated);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 8px;
      padding: 1.5rem;
      color: var(--text-primary);
      font-family: var(--font-sans);
      white-space: pre-wrap;
      line-height: 1.7;
      font-size: 1rem;
    }
    .play-cta-box {
      margin-top: 3rem;
      background: linear-gradient(135deg, var(--surface-secondary-emerald) 0%, var(--surface-secondary-walnut) 100%);
      border: 1px solid var(--accent-gold-primary);
      border-radius: 12px;
      padding: 2rem;
      display: flex;
      flex-wrap: wrap;
      justify-content: space-between;
      align-items: center;
      gap: 1.5rem;
    }
    .play-cta-box__content h3 {
      font-family: var(--font-display);
      font-size: 1.4rem;
      color: var(--text-primary);
      margin: 0 0 0.5rem 0;
    }
    .play-cta-box__content p {
      color: var(--text-muted);
      margin: 0;
      font-size: 0.95rem;
    }
    .btn-cta-gold {
      display: inline-flex;
      align-items: center;
      background-color: var(--accent-gold-primary);
      color: var(--accent-gold-contrast-text);
      font-weight: 700;
      padding: 0.85rem 1.5rem;
      border-radius: 8px;
      text-decoration: none;
      transition: background-color 0.2s;
    }
    .btn-cta-gold:hover {
      background-color: var(--accent-gold-hover);
    }
    .loading-state, .not-found-state {
      text-align: center;
      padding: 5rem 1rem;
      color: var(--text-muted);
    }
    .spinner {
      width: 40px;
      height: 40px;
      border: 3px solid var(--border-secondary-subtle);
      border-top-color: var(--accent-gold-primary);
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
      margin: 0 auto 1rem;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class GameDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly dataService = inject(ResilientDataService);
  private readonly seoMetaService = inject(SeoMetaService);
  private readonly jsonLdService = inject(JsonLdSchemaService);

  public readonly isLoading = signal<boolean>(true);
  public readonly game = signal<BoardGameModel | null>(null);

  ngOnInit(): void {
    const slug = this.route.snapshot.paramMap.get('slug');
    if (!slug) {
      this.isLoading.set(false);
      return;
    }

    this.dataService.getGameBySlug(slug).subscribe({
      next: (found) => {
        this.game.set(found);
        this.isLoading.set(false);

        if (found) {
          this.seoMetaService.setMeta({
            title: `${found.title} Kuralları ve Oynanış Rehberi`,
            description: found.tldrSummary,
            canonicalUrl: `https://masabasikulubu.com/oyunlar/${found.slug}`
          });

          this.jsonLdService.setSchema([
            this.jsonLdService.buildOrganizationSchema(),
            this.jsonLdService.buildGameSchema(found)
          ]);
        }
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  public getCategoryLabel(cat: string): string {
    const map: Record<string, string> = {
      BoardGame: 'Kutu Oyunu',
      SocialDeduction: 'Sosyal Çıkarım & Parti',
      FRP_DnD: 'FRP / D&D 5e',
      CardGame: 'Kart Oyunu'
    };
    return map[cat] ?? cat;
  }
}
