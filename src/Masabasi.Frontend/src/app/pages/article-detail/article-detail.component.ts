import { Component, ChangeDetectionStrategy, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';
import { ArticleModel } from '../../core/models/article.model';
import { TldrAiBoxComponent } from '../../shared/components/tldr-ai-box/tldr-ai-box.component';

@Component({
  selector: 'app-article-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, TldrAiBoxComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="article-detail-page container">
      @if (isLoading()) {
        <div class="loading-state">
          <div class="spinner"></div>
          <p>Yazı yükleniyor...</p>
        </div>
      } @else if (article(); as art) {
        <!-- Gezinme Çubuğu (Breadcrumb) -->
        <nav class="breadcrumb" aria-label="Gezinme Yolu">
          <a routerLink="/blog">Rehber & Blog</a>
          <span class="separator">/</span>
          <span class="current-cluster">{{ getClusterLabel(art.topicCluster) }}</span>
        </nav>

        <article class="article-body">
          <header class="article-header">
            <span class="cluster-badge">{{ getClusterLabel(art.topicCluster) }}</span>
            <h1 class="article-header__title">{{ art.title }}</h1>

            <div class="article-meta">
              <span class="author">✍️ {{ art.authorName }}</span>
              <span class="date">{{ art.publishedAtUtc | date:'dd MMMM yyyy':'UTC':'tr-TR' }}</span>
              <span class="read-time">⏱️ {{ art.estimatedReadTimeMinutes }} dakika okuma</span>
            </div>

            <!-- GEO ve AI Answer-First Kutusu -->
            <app-tldr-ai-box [summary]="art.tldrSummary" />
          </header>

          <!-- İçerik Alanı -->
          <section class="article-content" [innerHTML]="art.contentHtml"></section>

          <!-- Varsa Sıkça Sorulan Sorular Bölümü -->
          @if (faqs().length > 0) {
            <section class="article-faqs">
              <h2>Sıkça Sorulan Sorular</h2>
              <div class="faqs-list">
                @for (faq of faqs(); track faq.question) {
                  <div class="faq-item">
                    <h3>{{ faq.question }}</h3>
                    <p>{{ faq.answer }}</p>
                  </div>
                }
              </div>
            </section>
          }

          <!-- İlgili Cumartesi Etkinliği CTA'sı -->
          <section class="article-cta">
            <div class="article-cta__content">
              <h3>Topluluğumuza Katılın</h3>
              <p>
                Bu rehberdeki oyunları ve sistemleri her Cumartesi İstanbul'daki fiziksel buluşmalarımızda
                birlikte oynuyoruz.
              </p>
            </div>
            <a routerLink="/etkinlikler" class="btn-cta-gold">
              Cumartesi Etkinliğini İncele →
            </a>
          </section>
        </article>
      } @else {
        <div class="not-found-state">
          <h2>Makale Bulunamadı</h2>
          <p>Aradığınız rehber veya blog yazısı bulunamadı veya yayından kaldırılmış olabilir.</p>
          <a routerLink="/blog" class="btn-cta-gold">Blog Ana Sayfasına Dön</a>
        </div>
      }
    </main>
  `,
  styles: [`
    .article-detail-page {
      padding: 2rem 1rem 5rem;
      max-width: 820px;
      margin: 0 auto;
    }
    .breadcrumb {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.9rem;
      color: var(--text-muted);
      margin-bottom: 1.5rem;
    }
    .breadcrumb a {
      color: var(--accent-gold-primary);
      text-decoration: none;
    }
    .breadcrumb a:hover {
      text-decoration: underline;
    }
    .article-header {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
      margin-bottom: 2.5rem;
    }
    .cluster-badge {
      align-self: flex-start;
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
      font-size: 0.8rem;
      font-weight: 700;
      padding: 0.25rem 0.75rem;
      border-radius: 6px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .article-header__title {
      font-family: var(--font-display);
      font-size: clamp(2rem, 4.5vw, 3rem);
      color: var(--text-primary);
      line-height: 1.25;
      margin: 0;
    }
    .article-meta {
      display: flex;
      flex-wrap: wrap;
      gap: 1.25rem;
      font-size: 0.9rem;
      color: var(--text-muted);
      border-bottom: 1px solid var(--border-secondary-subtle);
      padding-bottom: 1rem;
    }
    .article-content {
      color: var(--text-primary);
      font-size: 1.1rem;
      line-height: 1.8;
      margin-bottom: 3rem;
    }
    .article-content h2, .article-content h3 {
      font-family: var(--font-display);
      color: var(--accent-gold-primary);
      margin: 2rem 0 1rem;
    }
    .article-content p {
      margin-bottom: 1.5rem;
    }
    .article-faqs {
      margin-top: 3rem;
      background-color: var(--bg-dominant-elevated);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 10px;
      padding: 1.75rem;
    }
    .article-faqs h2 {
      font-family: var(--font-display);
      font-size: 1.5rem;
      color: var(--accent-gold-primary);
      margin: 0 0 1.25rem 0;
    }
    .faqs-list {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }
    .faq-item h3 {
      font-size: 1.05rem;
      color: var(--text-primary);
      margin: 0 0 0.35rem 0;
    }
    .faq-item p {
      color: var(--text-muted);
      font-size: 0.95rem;
      line-height: 1.5;
      margin: 0;
    }
    .article-cta {
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
    .article-cta__content h3 {
      font-family: var(--font-display);
      font-size: 1.4rem;
      color: var(--text-primary);
      margin: 0 0 0.5rem 0;
    }
    .article-cta__content p {
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
export class ArticleDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly dataService = inject(ResilientDataService);
  private readonly seoMetaService = inject(SeoMetaService);
  private readonly jsonLdService = inject(JsonLdSchemaService);

  public readonly isLoading = signal<boolean>(true);
  public readonly article = signal<ArticleModel | null>(null);
  public readonly faqs = signal<Array<{ question: string; answer: string }>>([]);

  ngOnInit(): void {
    const slug = this.route.snapshot.paramMap.get('slug');
    if (!slug) {
      this.isLoading.set(false);
      return;
    }

    this.dataService.getArticleBySlug(slug).subscribe({
      next: (art) => {
        this.article.set(art);
        this.isLoading.set(false);

        if (art) {
          this.seoMetaService.setMeta({
            title: art.title,
            description: art.metaDescription || art.tldrSummary,
            ogType: 'article',
            canonicalUrl: `https://masabasikulubu.com/blog/${art.topicCluster}/${art.slug}`
          });

          const schemas: Record<string, unknown>[] = [
            this.jsonLdService.buildOrganizationSchema(),
            this.jsonLdService.buildArticleSchema(art)
          ];

          if (art.faqJson) {
            try {
              const parsedFaqs = JSON.parse(art.faqJson);
              this.faqs.set(parsedFaqs);
              schemas.push(this.jsonLdService.buildFaqSchema(parsedFaqs));
            } catch {
              // FAQ parse hatası sessizce yutulur
            }
          }

          this.jsonLdService.setSchema(schemas);
        }
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
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
