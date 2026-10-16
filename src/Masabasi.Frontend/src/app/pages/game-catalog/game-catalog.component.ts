import { Component, ChangeDetectionStrategy, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';
import { BoardGameModel } from '../../core/models/board-game.model';
import { TldrAiBoxComponent } from '../../shared/components/tldr-ai-box/tldr-ai-box.component';

@Component({
  selector: 'app-game-catalog',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TldrAiBoxComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="catalog-page container">
      <!-- Başlık ve GEO Özet Alanı -->
      <header class="catalog-hero">
        <span class="badge badge--emerald">📚 Kulüp Arşivi & Kurallar</span>
        <h1 class="catalog-hero__title">Oyun Kütüphanemiz</h1>
        <p class="catalog-hero__desc">
          Masabaşı Oyun Kulübü'nde her Cumartesi oynanan kutu oyunları, sosyal çıkarım klasikleri
          ve masaüstü rol yapma sistemlerinin Türkçe kural ve taktik kılavuzları.
        </p>

        <app-tldr-ai-box
          summary="Masabaşı Oyun Kulübü kütüphanesinde Catan, Secret Hitler, Avalon, Carcassonne gibi popüler kutu oyunları ve Dungeons & Dragons 5e FRP sistemleri yer alır. Etkinliklerimizde kurallar sıfırdan anlatılır."
        />
      </header>

      <!-- Arama ve Kategori Filtreleri -->
      <section class="catalog-controls" aria-label="Kütüphane Filtreleri">
        <div class="search-box">
          <input
            type="search"
            class="search-box__input"
            placeholder="Oyun adı veya sistem ara (ör. Catan, Secret Hitler)..."
            [ngModel]="searchQuery()"
            (ngModelChange)="searchQuery.set($event)"
            aria-label="Kütüphanede oyun ara"
          />
        </div>

        <div class="filter-tabs" role="tablist" aria-label="Oyun Kategorileri">
          <button
            type="button"
            class="filter-tab"
            [class.filter-tab--active]="selectedCategory() === 'ALL'"
            (click)="selectedCategory.set('ALL')">
            Tüm Oyunlar ({{ allGames().length }})
          </button>
          <button
            type="button"
            class="filter-tab"
            [class.filter-tab--active]="selectedCategory() === 'BoardGame'"
            (click)="selectedCategory.set('BoardGame')">
            Kutu Oyunları
          </button>
          <button
            type="button"
            class="filter-tab"
            [class.filter-tab--active]="selectedCategory() === 'SocialDeduction'"
            (click)="selectedCategory.set('SocialDeduction')">
            Sosyal Çıkarım & Parti
          </button>
          <button
            type="button"
            class="filter-tab"
            [class.filter-tab--active]="selectedCategory() === 'FRP_DnD'"
            (click)="selectedCategory.set('FRP_DnD')">
            FRP / D&D 5e
          </button>
        </div>
      </section>

      <!-- Oyun Listesi / Grid -->
      @if (isLoading()) {
        <div class="loading-state" aria-live="polite">
          <div class="spinner"></div>
          <p>Oyun kütüphanesi yükleniyor...</p>
        </div>
      } @else if (filteredGames().length === 0) {
        <div class="empty-state">
          <p>Arama kriterlerinize uygun oyun bulunamadı.</p>
          <button type="button" class="btn-reset" (click)="resetFilters()">
            Filtreleri Temizle
          </button>
        </div>
      } @else {
        <section class="games-grid" aria-label="Oyun Listesi">
          @for (game of filteredGames(); track game.id) {
            <article class="game-card">
              <header class="game-card__header">
                <span class="game-card__category">{{ getCategoryLabel(game.category) }}</span>
                @if (game.isInClubInventory) {
                  <span class="game-card__badge-inv">Envanterde</span>
                }
              </header>

              <div class="game-card__body">
                <h2 class="game-card__title">
                  <a [routerLink]="['/oyunlar', game.slug]">{{ game.title }}</a>
                </h2>
                <p class="game-card__summary">{{ game.tldrSummary }}</p>

                <div class="game-card__meta">
                  <div class="meta-tag">
                    <span aria-hidden="true">👥</span>
                    <span>{{ game.minPlayers }}-{{ game.maxPlayers }} Oyuncu</span>
                  </div>
                  <div class="meta-tag">
                    <span aria-hidden="true">⏱️</span>
                    <span>{{ game.playTimeMinutes }} dk</span>
                  </div>
                  <div class="meta-tag">
                    <span aria-hidden="true">🧠</span>
                    <span>Zorluk: {{ game.complexityRating }} / 5</span>
                  </div>
                </div>
              </div>

              <footer class="game-card__footer">
                <a [routerLink]="['/oyunlar', game.slug]" class="btn-read-rules">
                  Kuralları Oku & İncele →
                </a>
              </footer>
            </article>
          }
        </section>
      }
    </main>
  `,
  styles: [`
    .catalog-page {
      padding: 2.5rem 1rem 4rem;
      max-width: 1200px;
      margin: 0 auto;
    }
    .catalog-hero {
      text-align: center;
      margin-bottom: 3rem;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
    }
    .catalog-hero__title {
      font-family: var(--font-display);
      font-size: clamp(2rem, 4vw, 3rem);
      color: var(--text-primary);
      margin: 0;
    }
    .catalog-hero__desc {
      max-width: 700px;
      color: var(--text-muted);
      font-size: 1.125rem;
      line-height: 1.6;
      margin: 0;
    }
    .badge {
      display: inline-block;
      padding: 0.35rem 0.85rem;
      border-radius: 999px;
      font-size: 0.875rem;
      font-weight: 600;
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
    }
    .catalog-controls {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
      margin-bottom: 2.5rem;
    }
    .search-box__input {
      width: 100%;
      padding: 0.9rem 1.25rem;
      border-radius: 10px;
      border: 1px solid var(--border-secondary-subtle);
      background-color: var(--bg-dominant-elevated);
      color: var(--text-primary);
      font-size: 1rem;
      transition: border-color 0.2s;
    }
    .search-box__input:focus {
      outline: none;
      border-color: var(--accent-gold-primary);
    }
    .filter-tabs {
      display: flex;
      flex-wrap: wrap;
      gap: 0.6rem;
    }
    .filter-tab {
      background: var(--bg-dominant-elevated);
      border: 1px solid var(--border-secondary-subtle);
      color: var(--text-muted);
      padding: 0.6rem 1.1rem;
      border-radius: 8px;
      font-weight: 600;
      font-size: 0.9rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    .filter-tab:hover {
      border-color: var(--accent-gold-primary);
      color: var(--text-primary);
    }
    .filter-tab--active {
      background-color: var(--surface-secondary-emerald);
      border-color: var(--accent-gold-primary);
      color: var(--accent-gold-primary);
    }
    .games-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 1.5rem;
    }
    .game-card {
      background-color: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 12px;
      padding: 1.5rem;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      gap: 1.25rem;
      transition: transform 0.2s, border-color 0.2s;
    }
    .game-card:hover {
      transform: translateY(-3px);
      border-color: var(--accent-gold-primary);
    }
    .game-card__header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .game-card__category {
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
      font-size: 0.8rem;
      font-weight: 700;
      padding: 0.25rem 0.6rem;
      border-radius: 4px;
    }
    .game-card__badge-inv {
      font-size: 0.75rem;
      color: var(--accent-success-seat);
      font-weight: 600;
    }
    .game-card__title {
      font-family: var(--font-display);
      font-size: 1.25rem;
      margin: 0.5rem 0;
    }
    .game-card__title a {
      color: var(--text-primary);
      text-decoration: none;
    }
    .game-card__title a:hover {
      color: var(--accent-gold-primary);
    }
    .game-card__summary {
      color: var(--text-muted);
      font-size: 0.925rem;
      line-height: 1.5;
      margin: 0 0 1rem 0;
    }
    .game-card__meta {
      display: flex;
      flex-wrap: wrap;
      gap: 0.75rem;
      font-size: 0.85rem;
      color: var(--text-muted);
    }
    .meta-tag {
      display: inline-flex;
      align-items: center;
      gap: 0.35rem;
      background-color: var(--bg-dominant-primary);
      padding: 0.25rem 0.6rem;
      border-radius: 6px;
    }
    .btn-read-rules {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 100%;
      padding: 0.75rem;
      background-color: transparent;
      border: 1px solid var(--accent-gold-primary);
      color: var(--accent-gold-primary);
      border-radius: 8px;
      font-weight: 700;
      text-decoration: none;
      transition: all 0.2s;
    }
    .btn-read-rules:hover {
      background-color: var(--accent-gold-primary);
      color: var(--accent-gold-contrast-text);
    }
    .loading-state, .empty-state {
      text-align: center;
      padding: 4rem 1rem;
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
    .btn-reset {
      margin-top: 1rem;
      background: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
      border: 1px solid var(--accent-gold-primary);
      padding: 0.6rem 1.2rem;
      border-radius: 8px;
      font-weight: 600;
      cursor: pointer;
    }
  `]
})
export class GameCatalogComponent implements OnInit {
  private readonly dataService = inject(ResilientDataService);
  private readonly seoMetaService = inject(SeoMetaService);
  private readonly jsonLdService = inject(JsonLdSchemaService);

  public readonly isLoading = signal<boolean>(true);
  public readonly allGames = signal<BoardGameModel[]>([]);
  public readonly searchQuery = signal<string>('');
  public readonly selectedCategory = signal<string>('ALL');

  public readonly filteredGames = computed(() => {
    const query = this.searchQuery().trim().toLowerCase();
    const cat = this.selectedCategory();
    let games = this.allGames();

    if (cat !== 'ALL') {
      games = games.filter((g) => g.category === cat);
    }

    if (query) {
      games = games.filter(
        (g) =>
          g.title.toLowerCase().includes(query) ||
          g.tldrSummary.toLowerCase().includes(query)
      );
    }

    return games;
  });

  ngOnInit(): void {
    this.seoMetaService.setMeta({
      title: 'Kutu Oyunları Kütüphanesi & Türkçe Kural Rehberleri',
      description:
        "Masabaşı Oyun Kulübü'nde oynanan Catan, Secret Hitler, Avalon, D&D 5e ve diğer masaüstü oyunların Türkçe kural kılavuzları, oyuncu sayıları ve zorluk puanları.",
      canonicalUrl: 'https://masabasikulubu.com/oyun-kutuphanesi'
    });

    this.dataService.getGames().subscribe({
      next: (games) => {
        this.allGames.set(games);
        this.isLoading.set(false);
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

  public resetFilters(): void {
    this.searchQuery.set('');
    this.selectedCategory.set('ALL');
  }
}
