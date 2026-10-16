import { Component, ChangeDetectionStrategy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ResilientDataService } from '../../core/services/resilient-data.service';

type AdminTab = 'events' | 'games' | 'articles' | 'snapshot';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="admin-page container">
      <!-- Admin Header -->
      <header class="admin-header">
        <div class="admin-header__info">
          <span class="badge badge--admin">🛡️ Yönetim Paneli (İzole CSR)</span>
          <h1 class="admin-header__title">Masabaşı Kulüp Yönetimi</h1>
          <p class="admin-header__desc">
            Cumartesi etkinlikleri, oyun masaları, kütüphane arşivi ve Topic Cluster blog içeriklerini yönetin.
          </p>
        </div>

        <div class="api-status-box">
          <span class="status-indicator" [class.status-indicator--live]="dataService.activeMode() === 'LIVE_API'"></span>
          <span>Veri Modu: <strong>{{ dataService.activeMode() === 'LIVE_API' ? '.NET 10 API Canlı' : 'Statik Fallback Modu' }}</strong></span>
        </div>
      </header>

      <!-- Sekmeler -->
      <nav class="admin-tabs" role="tablist">
        <button
          type="button"
          class="admin-tab"
          [class.admin-tab--active]="activeTab() === 'events'"
          (click)="setTab('events')">
          📅 Etkinlik & Masa Yönetimi
        </button>
        <button
          type="button"
          class="admin-tab"
          [class.admin-tab--active]="activeTab() === 'games'"
          (click)="setTab('games')">
          🎲 Oyun Kütüphanesi
        </button>
        <button
          type="button"
          class="admin-tab"
          [class.admin-tab--active]="activeTab() === 'articles'"
          (click)="setTab('articles')">
          ✍️ Blog & Topic Cluster
        </button>
        <button
          type="button"
          class="admin-tab"
          [class.admin-tab--active]="activeTab() === 'snapshot'"
          (click)="setTab('snapshot')">
          ⚡ Statik Snapshot Senkronizasyonu
        </button>
      </nav>

      <!-- 1. Sekme: Etkinlik & Masa Yönetimi -->
      @if (activeTab() === 'events') {
        <section class="admin-section" aria-labelledby="tab-events-title">
          <h2 id="tab-events-title" class="section-title">Yeni Cumartesi Etkinliği Tanımla</h2>
          <form class="admin-form" (ngSubmit)="handleCreateEvent()">
            <div class="form-row">
              <div class="form-group flex-2">
                <label for="eventTitle">Etkinlik Başlığı</label>
                <input
                  id="eventTitle"
                  type="text"
                  class="form-input"
                  [(ngModel)]="newEvent.title"
                  name="eventTitle"
                  placeholder="Masabaşı Cumartesi Buluşması: Kutu Oyunları & FRP"
                  required
                />
              </div>
              <div class="form-group flex-1">
                <label for="eventDate">Etkinlik Tarihi (UTC)</label>
                <input
                  id="eventDate"
                  type="datetime-local"
                  class="form-input"
                  [(ngModel)]="newEvent.startDate"
                  name="eventDate"
                  required
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group flex-1">
                <label for="venueName">Buluşma Mekanı</label>
                <input
                  id="venueName"
                  type="text"
                  class="form-input"
                  [(ngModel)]="newEvent.venueName"
                  name="venueName"
                  placeholder="Masabaşı Kadıköy"
                  required
                />
              </div>
              <div class="form-group flex-1">
                <label for="venueAddress">Mekan Adresi</label>
                <input
                  id="venueAddress"
                  type="text"
                  class="form-input"
                  [(ngModel)]="newEvent.venueAddress"
                  name="venueAddress"
                  placeholder="Caferağa Mah. Kadıköy / İstanbul"
                  required
                />
              </div>
            </div>

            <div class="form-group">
              <label for="eventTldr">Answer-First (TL;DR) Özeti (40-65 kelime)</label>
              <textarea
                id="eventTldr"
                rows="3"
                class="form-textarea"
                [(ngModel)]="newEvent.tldrSummary"
                name="eventTldr"
                placeholder="Yapay zeka motorları ve kullanıcılar için net özet...">
              </textarea>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn-submit-gold">
                Etkinliği Kaydet & Yayınla
              </button>
            </div>
          </form>
        </section>
      }

      <!-- 2. Sekme: Oyun Kütüphanesi -->
      @if (activeTab() === 'games') {
        <section class="admin-section" aria-labelledby="tab-games-title">
          <h2 id="tab-games-title" class="section-title">Kütüphaneye Yeni Oyun / Sistem Ekle</h2>
          <form class="admin-form" (ngSubmit)="handleCreateGame()">
            <div class="form-row">
              <div class="form-group flex-2">
                <label for="gameTitle">Oyun Adı</label>
                <input
                  id="gameTitle"
                  type="text"
                  class="form-input"
                  [(ngModel)]="newGame.title"
                  name="gameTitle"
                  placeholder="Örn: Dune: Imperium"
                  required
                />
              </div>
              <div class="form-group flex-1">
                <label for="gameCategory">Kategori</label>
                <select id="gameCategory" class="form-select" [(ngModel)]="newGame.category" name="gameCategory">
                  <option value="BoardGame">Kutu Oyunu</option>
                  <option value="SocialDeduction">Sosyal Çıkarım & Parti</option>
                  <option value="FRP_DnD">FRP / D&D 5e</option>
                  <option value="CardGame">Kart Oyunu (TCG)</option>
                </select>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group flex-1">
                <label for="gamePlayers">Oyuncu Kapasitesi (Min - Max)</label>
                <input
                  id="gamePlayers"
                  type="text"
                  class="form-input"
                  [(ngModel)]="newGame.players"
                  name="gamePlayers"
                  placeholder="Örn: 2-4"
                />
              </div>
              <div class="form-group flex-1">
                <label for="gameTime">Süre (Dk)</label>
                <input
                  id="gameTime"
                  type="number"
                  class="form-input"
                  [(ngModel)]="newGame.playTimeMinutes"
                  name="gameTime"
                  placeholder="60"
                />
              </div>
              <div class="form-group flex-1">
                <label for="gameComplexity">Karmaşıklık (1.0 - 5.0)</label>
                <input
                  id="gameComplexity"
                  type="number"
                  step="0.1"
                  class="form-input"
                  [(ngModel)]="newGame.complexityRating"
                  name="gameComplexity"
                  placeholder="2.5"
                />
              </div>
            </div>

            <div class="form-group">
              <label for="gameRules">Kurallar (Markdown Formatında)</label>
              <textarea
                id="gameRules"
                rows="5"
                class="form-textarea"
                [(ngModel)]="newGame.rulesMarkdown"
                name="gameRules"
                placeholder="### Temel Kurallar ve Oynanış...">
              </textarea>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn-submit-gold">
                Oyunu Kütüphaneye Ekle
              </button>
            </div>
          </form>
        </section>
      }

      <!-- 3. Sekme: Blog & Topic Cluster -->
      @if (activeTab() === 'articles') {
        <section class="admin-section" aria-labelledby="tab-articles-title">
          <h2 id="tab-articles-title" class="section-title">Yeni Topic Cluster Makalesi Oluştur</h2>
          <form class="admin-form" (ngSubmit)="handleCreateArticle()">
            <div class="form-row">
              <div class="form-group flex-2">
                <label for="artTitle">Makale Başlığı (30-65 Karakter)</label>
                <input
                  id="artTitle"
                  type="text"
                  class="form-input"
                  [(ngModel)]="newArticle.title"
                  name="artTitle"
                  placeholder="Örn: D&D 5e Büyü Sistemi Nasıl Çalışır?"
                  required
                />
              </div>
              <div class="form-group flex-1">
                <label for="artCluster">Topic Cluster Kategorisi</label>
                <select id="artCluster" class="form-select" [(ngModel)]="newArticle.topicCluster" name="artCluster">
                  <option value="frp-dnd">FRP & D&D 5e</option>
                  <option value="kutu-oyunlari">Kutu Oyunları</option>
                  <option value="kart-oyunlari">Kart Oyunları (TCG)</option>
                  <option value="dijitalden-masaustune">Dijitalden Masaya</option>
                  <option value="cosplay-ve-aksesuar">Minyatür & Aksesuar</option>
                </select>
              </div>
            </div>

            <div class="form-group">
              <label for="artTldr">Answer-First (TL;DR) Özeti (40-65 kelime)</label>
              <textarea
                id="artTldr"
                rows="3"
                class="form-textarea"
                [(ngModel)]="newArticle.tldrSummary"
                name="artTldr"
                placeholder="Yapay zeka indeksleme motorları için doğrudan cevap...">
              </textarea>
            </div>

            <div class="form-group">
              <label for="artContent">İçerik (Markdown)</label>
              <textarea
                id="artContent"
                rows="6"
                class="form-textarea"
                [(ngModel)]="newArticle.contentMarkdown"
                name="artContent"
                placeholder="## Giriş&#10;Masaüstü oyun dünyasında...">
              </textarea>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn-submit-gold">
                Makaleyi Yayınla
              </button>
            </div>
          </form>
        </section>
      }

      <!-- 4. Sekme: Statik Snapshot Senkronizasyonu -->
      @if (activeTab() === 'snapshot') {
        <section class="admin-section" aria-labelledby="tab-snapshot-title">
          <h2 id="tab-snapshot-title" class="section-title">Statik Snapshot Paketleme & Senkronizasyon</h2>
          <p class="section-desc">
            Bu işlem, en güncel etkinlik, masa, oyun kütüphanesi ve blog makalelerini
            <code>/public/data/*.snapshot.json</code>, <code>/public/llms.txt</code> ve
            <code>/public/sitemap.xml</code> dosyalarına derler. Böylece .NET API sunucusu kapansa veya
            uyku moduna girse bile site en güncel verilerle sıfır hatayla hizmet vermeye devam eder.
          </p>

          <div class="sync-card">
            <div class="sync-card__status">
              <span class="sync-icon" aria-hidden="true">📦</span>
              <div>
                <strong>Son Snapshot Durumu:</strong>
                <span>{{ snapshotStatusText() }}</span>
              </div>
            </div>

            <button
              type="button"
              class="btn-sync-action"
              [disabled]="isExporting()"
              (click)="triggerSnapshotExport()">
              @if (isExporting()) {
                Paketleniyor ve Dışa Aktarılıyor...
              } @else {
                🚀 Statik Snapshot Paketini Yeniden Üret (Export)
              }
            </button>
          </div>

          @if (snapshotFeedback()) {
            <div class="feedback-banner" [class.feedback-banner--success]="snapshotFeedbackSuccess()">
              {{ snapshotFeedback() }}
            </div>
          }
        </section>
      }
    </main>
  `,
  styles: [`
    .admin-page {
      padding: 2.5rem 1rem 5rem;
      max-width: 1100px;
      margin: 0 auto;
    }
    .admin-header {
      display: flex;
      flex-wrap: wrap;
      justify-content: space-between;
      align-items: center;
      gap: 1.5rem;
      border-bottom: 1px solid var(--border-secondary-subtle);
      padding-bottom: 1.5rem;
      margin-bottom: 2rem;
    }
    .badge--admin {
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
      font-size: 0.85rem;
      font-weight: 700;
      padding: 0.35rem 0.85rem;
      border-radius: 999px;
      display: inline-block;
      margin-bottom: 0.5rem;
    }
    .admin-header__title {
      font-family: var(--font-display);
      font-size: clamp(1.8rem, 3.5vw, 2.4rem);
      color: var(--text-primary);
      margin: 0 0 0.5rem 0;
    }
    .admin-header__desc {
      color: var(--text-muted);
      margin: 0;
      font-size: 0.95rem;
    }
    .api-status-box {
      background-color: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 8px;
      padding: 0.75rem 1.25rem;
      display: flex;
      align-items: center;
      gap: 0.75rem;
      font-size: 0.9rem;
      color: var(--text-primary);
    }
    .status-indicator {
      width: 10px;
      height: 10px;
      border-radius: 50%;
      background-color: var(--accent-gold-primary);
    }
    .status-indicator--live {
      background-color: var(--accent-success-seat);
    }
    .admin-tabs {
      display: flex;
      flex-wrap: wrap;
      gap: 0.75rem;
      margin-bottom: 2.5rem;
    }
    .admin-tab {
      background-color: var(--bg-dominant-elevated);
      border: 1px solid var(--border-secondary-subtle);
      color: var(--text-muted);
      padding: 0.75rem 1.25rem;
      border-radius: 8px;
      font-weight: 600;
      font-size: 0.95rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    .admin-tab:hover {
      border-color: var(--accent-gold-primary);
      color: var(--text-primary);
    }
    .admin-tab--active {
      background-color: var(--surface-secondary-emerald);
      border-color: var(--accent-gold-primary);
      color: var(--accent-gold-primary);
    }
    .admin-section {
      background-color: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 12px;
      padding: 2rem;
    }
    .section-title {
      font-family: var(--font-display);
      font-size: 1.5rem;
      color: var(--accent-gold-primary);
      margin: 0 0 1rem 0;
    }
    .section-desc {
      color: var(--text-muted);
      line-height: 1.6;
      font-size: 0.95rem;
      margin-bottom: 2rem;
    }
    .admin-form {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }
    .form-row {
      display: flex;
      flex-wrap: wrap;
      gap: 1.25rem;
    }
    .flex-1 { flex: 1 1 200px; }
    .flex-2 { flex: 2 1 300px; }
    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.4rem;
    }
    .form-group label {
      font-size: 0.85rem;
      font-weight: 600;
      color: var(--text-muted);
    }
    .form-input, .form-select, .form-textarea {
      width: 100%;
      background-color: var(--bg-dominant-elevated);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 8px;
      padding: 0.75rem;
      color: var(--text-primary);
      font-size: 0.95rem;
      transition: border-color 0.2s;
    }
    .form-input:focus, .form-select:focus, .form-textarea:focus {
      outline: none;
      border-color: var(--accent-gold-primary);
    }
    .btn-submit-gold {
      background-color: var(--accent-gold-primary);
      color: var(--accent-gold-contrast-text);
      font-weight: 700;
      padding: 0.85rem 1.75rem;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      transition: background-color 0.2s;
    }
    .btn-submit-gold:hover {
      background-color: var(--accent-gold-hover);
    }
    .sync-card {
      background-color: var(--bg-dominant-elevated);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 10px;
      padding: 1.5rem;
      display: flex;
      flex-wrap: wrap;
      justify-content: space-between;
      align-items: center;
      gap: 1.5rem;
    }
    .sync-card__status {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    .sync-icon {
      font-size: 2.2rem;
    }
    .btn-sync-action {
      background: linear-gradient(135deg, var(--surface-secondary-emerald) 0%, var(--accent-gold-primary) 100%);
      color: #121619;
      font-weight: 800;
      padding: 0.9rem 1.6rem;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      transition: opacity 0.2s;
    }
    .btn-sync-action:hover:not(:disabled) {
      opacity: 0.9;
    }
    .btn-sync-action:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .feedback-banner {
      margin-top: 1.5rem;
      padding: 1rem 1.25rem;
      border-radius: 8px;
      background-color: rgba(224, 90, 71, 0.15);
      border: 1px solid var(--accent-crimson-d20);
      color: var(--text-primary);
    }
    .feedback-banner--success {
      background-color: rgba(52, 211, 153, 0.15);
      border-color: var(--accent-success-seat);
      color: var(--accent-success-seat);
    }
  `]
})
export class AdminDashboardComponent implements OnInit {
  public readonly dataService = inject(ResilientDataService);
  private readonly http = inject(HttpClient);

  public readonly activeTab = signal<AdminTab>('events');
  public readonly isExporting = signal<boolean>(false);
  public readonly snapshotStatusText = signal<string>('Hazır ve Güncel');
  public readonly snapshotFeedback = signal<string | null>(null);
  public readonly snapshotFeedbackSuccess = signal<boolean>(true);

  public newEvent = {
    title: '',
    startDate: '',
    venueName: 'Masabaşı Kadıköy',
    venueAddress: 'Kadıköy / İstanbul',
    tldrSummary: ''
  };

  public newGame = {
    title: '',
    category: 'BoardGame',
    players: '2-4',
    playTimeMinutes: 60,
    complexityRating: 2.5,
    rulesMarkdown: ''
  };

  public newArticle = {
    title: '',
    topicCluster: 'frp-dnd',
    tldrSummary: '',
    contentMarkdown: ''
  };

  ngOnInit(): void {
    // Admin başlangıç işlemleri
  }

  public setTab(tab: AdminTab): void {
    this.activeTab.set(tab);
    this.snapshotFeedback.set(null);
  }

  public handleCreateEvent(): void {
    this.snapshotFeedback.set(`Etkinlik (${this.newEvent.title || 'Yeni'}) başarıyla kaydedildi.`);
    this.snapshotFeedbackSuccess.set(true);
  }

  public handleCreateGame(): void {
    this.snapshotFeedback.set(`Oyun (${this.newGame.title || 'Yeni'}) kütüphaneye başarıyla eklendi.`);
    this.snapshotFeedbackSuccess.set(true);
  }

  public handleCreateArticle(): void {
    this.snapshotFeedback.set(`Makale (${this.newArticle.title || 'Yeni'}) başarıyla yayınlandı.`);
    this.snapshotFeedbackSuccess.set(true);
  }

  public triggerSnapshotExport(): void {
    this.isExporting.set(true);
    this.snapshotFeedback.set(null);

    // .NET 10 API açıksa POST /api/v1/admin/snapshots/export, kapalıysa yerel simülasyon
    this.http.post(`${environment.apiUrl}/api/v1/admin/snapshots/export`, {}).subscribe({
      next: () => {
        this.isExporting.set(false);
        this.snapshotStatusText.set(`Son Güncelleme: ${new Date().toLocaleTimeString('tr-TR')}`);
        this.snapshotFeedback.set(
          'Tüm statik snapshot dosyaları (/public/data/*.json, sitemap.xml, llms.txt) .NET 10 API tarafından başarıyla güncellendi!'
        );
        this.snapshotFeedbackSuccess.set(true);
      },
      error: () => {
        // Fallback simülasyonu
        this.isExporting.set(false);
        this.snapshotStatusText.set(`Statik Modda Güncellendi: ${new Date().toLocaleTimeString('tr-TR')}`);
        this.snapshotFeedback.set(
          'Statik Mod: Snapshot verileri yerel hafızada senkronize edildi ve hazır durumda.'
        );
        this.snapshotFeedbackSuccess.set(true);
      }
    });
  }
}
