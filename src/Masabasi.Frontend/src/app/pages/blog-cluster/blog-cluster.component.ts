import { Component, ChangeDetectionStrategy, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';
import { ArticleSummaryModel } from '../../core/models/article.model';
import { TldrAiBoxComponent } from '../../shared/components/tldr-ai-box/tldr-ai-box.component';

@Component({
  selector: 'app-blog-cluster',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, TldrAiBoxComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="blog-page container">
      <!-- Başlık ve Topic Cluster Açıklaması -->
      <header class="blog-hero">
        <span class="badge badge--emerald">📖 Masaüstü Oyun Kültürü & Rehberler</span>
        <h1 class="blog-hero__title">Masabaşı Rehber & Blog</h1>
        <p class="blog-hero__desc">
          Kutu oyunları, FRP & D&D 5e taktikleri, kart oyunları ve dijital RPG'lerden masaüstüne geçiş rehberleri.
        </p>

        <app-tldr-ai-box
          summary="Masabaşı Blog, masaüstü oyun kültürünü derinlemesine ele alan bir topic cluster merkezidir. D&D 5e kurallarından MTG Commander stratejilerine, Baldur's Gate 3 köprüsünden minyatür boyama atölyelerine uzanan güncel Türkçe içerikler sunar."
        />
      </header>

      <!-- Topic Cluster Kategori Sekmeleri -->
      <nav class="cluster-tabs" aria-label="Blog Konu Kümeleri">
        <button
          type="button"
          class="cluster-tab"
          [class.cluster-tab--active]="activeCluster() === 'ALL'"
          (click)="setCluster('ALL')">
          Tüm Yazılar ({{ allArticles().length }})
        </button>
        <button
          type="button"
          class="cluster-tab"
          [class.cluster-tab--active]="activeCluster() === 'frp-dnd'"
          (click)="setCluster('frp-dnd')">
          🧙 FRP & D&D 5e
        </button>
        <button
          type="button"
          class="cluster-tab"
          [class.cluster-tab--active]="activeCluster() === 'kutu-oyunlari'"
          (click)="setCluster('kutu-oyunlari')">
          🎲 Kutu Oyunları
        </button>
        <button
          type="button"
          class="cluster-tab"
          [class.cluster-tab--active]="activeCluster() === 'kart-oyunlari'"
          (click)="setCluster('kart-oyunlari')">
          🃏 Kart Oyunları (TCG)
        </button>
        <button
          type="button"
          class="cluster-tab"
          [class.cluster-tab--active]="activeCluster() === 'dijitalden-masaustune'"
          (click)="setCluster('dijitalden-masaustune')">
          🎮 Dijitalden Masaya (BG3)
        </button>
        <button
          type="button"
          class="cluster-tab"
          [class.cluster-tab--active]="activeCluster() === 'cosplay-ve-aksesuar'"
          (click)="setCluster('cosplay-ve-aksesuar')">
          🎨 Minyatür & Aksesuar
        </button>
      </nav>

      <!-- Makale Listesi Grid -->
      @if (isLoading()) {
        <div class="loading-state">
          <div class="spinner"></div>
          <p>Makaleler yükleniyor...</p>
        </div>
      } @else if (filteredArticles().length === 0) {
        <div class="empty-state">
          <p>Bu kategoride henüz yayınlanmış bir yazı bulunmuyor.</p>
          <button type="button" class="btn-reset" (click)="setCluster('ALL')">Tüm Yazıları Gör</button>
        </div>
      } @else {
        <section class="articles-grid" aria-label="Yazı Listesi">
          @for (article of filteredArticles(); track article.id) {
            <article class="article-card">
              <header class="article-card__header">
                <span class="cluster-badge">{{ getClusterLabel(article.topicCluster) }}</span>
                <span class="read-time">⏱️ {{ article.estimatedReadTimeMinutes }} dk okuma</span>
              </header>

              <div class="article-card__body">
                <h2 class="article-card__title">
                  <a [routerLink]="['/blog', article.topicCluster, article.slug]">
                    {{ article.title }}
                  </a>
                </h2>
                <p class="article-card__summary">{{ article.tldrSummary }}</p>
              </div>

              <footer class="article-card__footer">
                <span class="article-date">
                  {{ article.publishedAtUtc | date:'dd MMMM yyyy':'UTC':'tr-TR' }}
                </span>
                <a [routerLink]="['/blog', article.topicCluster, article.slug]" class="btn-read-more">
                  Devamını Oku →
                </a>
              </footer>
            </article>
          }
        </section>
      }
    </main>
  `,
  styles: [`
    .blog-page {
      padding: 2.5rem 1rem 5rem;
      max-width: 1100px;
      margin: 0 auto;
    }
    .blog-hero {
      text-align: center;
      margin-bottom: 2.5rem;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
    }
    .blog-hero__title {
      font-family: var(--font-display);
      font-size: clamp(2rem, 4vw, 3rem);
      color: var(--text-primary);
      margin: 0;
    }
    .blog-hero__desc {
      color: var(--text-muted);
      font-size: 1.125rem;
      max-width: 700px;
      margin: 0;
      line-height: 1.6;
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
    .cluster-tabs {
      display: flex;
      flex-wrap: wrap;
      justify-content: center;
      gap: 0.6rem;
      margin-bottom: 2.5rem;
    }
    .cluster-tab {
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
    .cluster-tab:hover {
      border-color: var(--accent-gold-primary);
      color: var(--text-primary);
    }
    .cluster-tab--active {
      background-color: var(--surface-secondary-emerald);
      border-color: var(--accent-gold-primary);
      color: var(--accent-gold-primary);
    }
    .articles-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 1.5rem;
    }
    .article-card {
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
    .article-card:hover {
      transform: translateY(-3px);
      border-color: var(--accent-gold-primary);
    }
    .article-card__header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .cluster-badge {
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
      font-size: 0.75rem;
      font-weight: 700;
      padding: 0.25rem 0.6rem;
      border-radius: 4px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .read-time {
      font-size: 0.8rem;
      color: var(--text-muted);
    }
    .article-card__title {
      font-family: var(--font-display);
      font-size: 1.25rem;
      margin: 0.5rem 0;
      line-height: 1.35;
    }
    .article-card__title a {
      color: var(--text-primary);
      text-decoration: none;
    }
    .article-card__title a:hover {
      color: var(--accent-gold-primary);
    }
    .article-card__summary {
      color: var(--text-muted);
      font-size: 0.925rem;
      line-height: 1.5;
      margin: 0;
    }
    .article-card__footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      border-top: 1px solid var(--border-secondary-subtle);
      padding-top: 1rem;
    }
    .article-date {
      font-size: 0.825rem;
      color: var(--text-muted);
    }
    .btn-read-more {
      color: var(--accent-gold-primary);
      text-decoration: none;
      font-weight: 700;
      font-size: 0.9rem;
    }
    .btn-read-more:hover {
      text-decoration: underline;
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
      padding: 0.5rem 1rem;
      border-radius: 6px;
      cursor: pointer;
    }
  `]
})
export class BlogClusterComponent implements OnInit {
  private readonly dataService = inject(ResilientDataService);
  private readonly seoMetaService = inject(SeoMetaService);
  private readonly route = inject(ActivatedRoute);

  public readonly isLoading = signal<boolean>(true);
  public readonly allArticles = signal<ArticleSummaryModel[]>([]);
  public readonly activeCluster = signal<string>('ALL');

  public readonly filteredArticles = computed(() => {
    const cluster = this.activeCluster().toLowerCase();
    const list = this.allArticles();
    if (cluster === 'all') {
      return list;
    }
    return list.filter((a) => a.topicCluster.toLowerCase() === cluster);
  });

  ngOnInit(): void {
    this.seoMetaService.setMeta({
      title: 'Masaüstü Oyun Kültürü & Rehberler | Topic Cluster',
      description:
        "Dungeons & Dragons, modern kutu oyunları, MTG kart stratejileri, Baldur's Gate 3 köprü yazıları ve masa aksesuarları hakkında rehber makaleler.",
      canonicalUrl: 'https://masabasikulubu.com/blog'
    });

    const categoryParam = this.route.snapshot.queryParamMap.get('kategori');
    if (categoryParam) {
      this.activeCluster.set(categoryParam);
    }

    this.dataService.getArticles().subscribe({
      next: (articles) => {
        this.allArticles.set(articles);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  public setCluster(cluster: string): void {
    this.activeCluster.set(cluster);
  }

  public getClusterLabel(cluster: string): string {
    const map: Record<string, string> = {
      'frp-dnd': 'FRP & D&D 5e',
      'kutu-oyunlari': 'Kutu Oyunları',
      'kart-oyunlari': 'Kart Oyunları (TCG)',
      'dijitalden-masaustune': 'Dijitalden Masaya',
      'cosplay-ve-aksesuar': 'Minyatür & Aksesuar'
    };
    return map[cluster.toLowerCase()] ?? cluster;
  }
}
