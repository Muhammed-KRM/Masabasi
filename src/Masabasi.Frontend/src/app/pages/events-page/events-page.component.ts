import { Component, ChangeDetectionStrategy, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { TableHubService } from '../../core/services/table-hub.service';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';
import { ClubEventModel, GameTableModel, ReservationResultModel } from '../../core/models/event-table.model';
import { TableSeatCardComponent } from '../../shared/components/table-seat-card/table-seat-card.component';
import { TldrAiBoxComponent } from '../../shared/components/tldr-ai-box/tldr-ai-box.component';
import { ReservationModalComponent } from '../../shared/components/reservation-modal/reservation-modal.component';

@Component({
  selector: 'app-events-page',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    TableSeatCardComponent,
    TldrAiBoxComponent,
    ReservationModalComponent
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="events-page container">
      @if (isLoading()) {
        <div class="events-page__loading" aria-live="polite">
          <div class="spinner"></div>
          <p>Masaüstü macerası ve masalar yükleniyor...</p>
        </div>
      } @else if (currentEvent(); as event) {
        <!-- Hero ve Başlık Bölümü -->
        <header class="events-hero">
          <div class="events-hero__badge-row">
            <span class="badge badge--emerald">📅 Bu Cumartesi Buluşması</span>
            <span class="badge badge--success">🎲 Giriş ve Masalar Açık</span>
          </div>

          <h1 class="events-hero__title">{{ event.title }}</h1>

          <div class="events-hero__meta">
            <div class="meta-item">
              <span class="meta-item__icon" aria-hidden="true">📍</span>
              <div>
                <strong>Mekan:</strong>
                <span>{{ event.venueName }} ({{ event.venueAddress }})</span>
              </div>
            </div>
            <div class="meta-item">
              <span class="meta-item__icon" aria-hidden="true">⏰</span>
              <div>
                <strong>Tarih & Saat:</strong>
                <span>{{ event.startDateUtc | date:'dd MMMM yyyy, EEEE HH:mm':'UTC':'tr-TR' }}</span>
              </div>
            </div>
          </div>

          <!-- GEO & AI Özet Kutusu -->
          <app-tldr-ai-box [summary]="event.tldrSummary" />
        </header>

        <!-- Masalar Bölümü: Yalnızca canlı API'den gelen gerçek masalar varsa göster -->
        @if (allTables().length > 0) {
          <section class="tables-section" aria-labelledby="tables-heading">
            <div class="tables-section__header">
              <div>
                <h2 id="tables-heading" class="tables-section__title">Açık Masalar & Kontenjan Durumu</h2>
                <p class="tables-section__subtitle">
                  İlgilendiğiniz masaya tıklayarak koltuğunuzu ayırtın veya oyun anlatıcısı DM'in detaylarını inceleyin.
                </p>
              </div>

              <!-- Kategori Filtre Butonları -->
              <div class="category-filters" role="group" aria-label="Masa Kategorileri">
                <button
                  type="button"
                  class="filter-chip"
                  [class.active]="selectedCategory() === 'ALL'"
                  (click)="setCategory('ALL')">
                  Tümü ({{ allTables().length }})
                </button>
                <button
                  type="button"
                  class="filter-chip"
                  [class.active]="selectedCategory() === 'FRP_DnD'"
                  (click)="setCategory('FRP_DnD')">
                  ⚔️ FRP / D&D 5e
                </button>
                <button
                  type="button"
                  class="filter-chip"
                  [class.active]="selectedCategory() === 'SocialDeduction'"
                  (click)="setCategory('SocialDeduction')">
                  🎭 Sosyal Çıkarım
                </button>
                <button
                  type="button"
                  class="filter-chip"
                  [class.active]="selectedCategory() === 'BoardGame'"
                  (click)="setCategory('BoardGame')">
                  ♟️ Kutu Oyunları
                </button>
              </div>
            </div>

            <!-- Masa Kartları Grid Listesi -->
            <div class="tables-grid">
              @for (table of filteredTables(); track table.id) {
                <app-table-seat-card
                  [table]="table"
                  [meetupUrl]="event.meetupEventUrl ?? 'https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/'"
                  (reserveClicked)="openReservationModal($event)" />
              }
            </div>
          </section>

          <!-- Koltuk Rezervasyon Modal Penceresi (Sadece gerçek masa listesi varken) -->
          @if (selectedTableForReservation(); as table) {
            <app-reservation-modal
              [table]="table"
              (closed)="closeReservationModal()"
              (reservationSuccess)="handleReservationSuccess($event)" />
          }
        } @else {
          <!-- Masalar Canlı Açılmamışsa veya Tanıtım Modundaysa: Dürüst ve Zengin Buluşma Rehberi -->
          <section class="walkin-section" aria-labelledby="walkin-title">
            <div class="walkin-card">
              <span class="walkin-eyebrow">Etkinlik Katılım Rehberi</span>
              <h2 id="walkin-title" class="walkin-card__title">
                Masalarımız Kadıköy Mekanında Doğrudan Açık Katılımla Kurulmaktadır
              </h2>
              <p class="walkin-card__lead">
                Masabaşı Oyun Kulübü haftalık buluşmalarında masalarımız, mekana gelen katılımcılarımızın
                oyun tercihlerine göre moderatörlerimiz eşliğinde anlık ve serbest akışla kurulur.
                Önceden koltuk rezervasyonu yapmanıza gerek yoktur.
              </p>

              <div class="walkin-steps">
                <div class="walkin-step">
                  <span class="walkin-step__icon" aria-hidden="true">🤝</span>
                  <div class="walkin-step__body">
                    <strong>1. Meetup veya Instagram'dan Katılımınızı Bildirin</strong>
                    <p>Haftalık buluşma sayfamızdan LCV vererek veya Instagram'dan bize mesaj atarak geleceğinizi iletebilirsiniz.</p>
                  </div>
                </div>

                <div class="walkin-step">
                  <span class="walkin-step__icon" aria-hidden="true">📍</span>
                  <div class="walkin-step__body">
                    <strong>2. Cumartesi Günü Saat 11:00 İtibarıyla Mekana Gelin</strong>
                    <p>Kadıköy'deki buluşma mekanımıza ister tek başınıza ister arkadaşlarınızla gelin; kapıda sizi karşılar ve açık masalara yönlendiririz.</p>
                  </div>
                </div>

                <div class="walkin-step">
                  <span class="walkin-step__icon" aria-hidden="true">🎲</span>
                  <div class="walkin-step__body">
                    <strong>3. Masaya Oturun, Kuralları Birlikte Dinleyelim</strong>
                    <p>Oyun kurallarını bilmiyorum diye endişelenmeyin! Her masadaki moderatörlerimiz veya deneyimli oyuncular kuralları sabırla anlatır.</p>
                  </div>
                </div>
              </div>

              <div class="walkin-actions">
                <a
                  [href]="event.meetupEventUrl ?? 'https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/'"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="btn-gold-action">
                  <span aria-hidden="true">🤝</span>
                  <span>Meetup Etkinliğine Katıl</span>
                </a>
                <a
                  [href]="event.externalRegistrationUrl ?? 'https://www.instagram.com/masabasikulubu/'"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="btn-outline-action">
                  <span aria-hidden="true">📸</span>
                  <span>Instagram Hesabımızdan Mesaj At</span>
                </a>
              </div>
            </div>
          </section>
        }
      } @else {
        <div class="empty-state">
          <h2>Yaklaşan Etkinlik Bulunamadı</h2>
          <p>Yeni Cumartesi etkinliğimiz yakında duyurulacaktır. Bizi Instagram ve Meetup üzerinden takip edebilirsiniz.</p>
        </div>
      }
    </main>
  `,
  styles: [`
    .events-page {
      padding-top: 2.5rem;
      padding-bottom: 4rem;
    }

    .events-page__loading {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 400px;
      gap: 1.25rem;
      color: var(--text-muted);
    }

    .spinner {
      width: 44px;
      height: 44px;
      border: 3px solid rgba(212, 175, 55, 0.2);
      border-top-color: var(--accent-gold-primary);
      border-radius: 50%;
      animation: spin 0.9s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .events-hero {
      margin-bottom: 3.5rem;
    }

    .events-hero__badge-row {
      display: flex;
      flex-wrap: wrap;
      gap: 0.75rem;
      margin-bottom: 1rem;
    }

    .events-hero__title {
      font-size: 2.25rem;
      color: var(--text-primary);
      line-height: 1.25;
      margin-bottom: 1.25rem;

      @media (min-width: 768px) {
        font-size: 2.85rem;
      }
    }

    .events-hero__meta {
      display: grid;
      grid-template-columns: 1fr;
      gap: 1rem;
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 12px;
      padding: 1.2rem 1.5rem;
      margin-bottom: 1.5rem;

      @media (min-width: 640px) {
        grid-template-columns: 1fr 1fr;
      }
    }

    .meta-item {
      display: flex;
      align-items: flex-start;
      gap: 0.75rem;
    }

    .meta-item__icon {
      font-size: 1.35rem;
      line-height: 1;
    }

    .meta-item strong {
      display: block;
      font-size: 0.8rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--accent-gold-primary);
    }

    .meta-item span {
      font-size: 0.95rem;
      color: var(--text-primary);
    }

    .tables-section {
      margin-top: 2rem;
    }

    .tables-section__header {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
      margin-bottom: 2rem;

      @media (min-width: 900px) {
        flex-direction: row;
        justify-content: space-between;
        align-items: flex-end;
      }
    }

    .tables-section__title {
      font-size: 1.85rem;
      margin-bottom: 0.35rem;
    }

    .tables-section__subtitle {
      font-size: 0.95rem;
      margin: 0;
    }

    .category-filters {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .filter-chip {
      background: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      color: var(--text-muted);
      font-family: var(--font-sans);
      font-size: 0.85rem;
      font-weight: 600;
      padding: 0.45rem 0.9rem;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        border-color: var(--accent-gold-primary);
        color: var(--text-primary);
      }

      &.active {
        background: var(--surface-secondary-emerald);
        border-color: var(--accent-gold-primary);
        color: var(--accent-gold-primary);
        box-shadow: 0 0 10px rgba(212, 175, 55, 0.2);
      }
    }

    .tables-grid {
      display: grid;
      grid-template-columns: 1fr;
      gap: 1.5rem;

      @media (min-width: 640px) {
        grid-template-columns: repeat(2, 1fr);
      }

      @media (min-width: 1024px) {
        grid-template-columns: repeat(3, 1fr);
      }
    }

    .empty-state {
      text-align: center;
      padding: 3rem 1.5rem;
      background: var(--surface-secondary-walnut);
      border: 1px dashed var(--border-secondary-subtle);
      border-radius: 12px;
      color: var(--text-muted);
    }

    /* Walk-in Etkinlik Katılım Rehberi Bölümü */
    .walkin-section {
      margin-top: 2rem;
    }

    .walkin-card {
      background: linear-gradient(145deg, var(--surface-secondary-walnut) 0%, #151922 100%);
      border: 1px solid var(--accent-gold-primary);
      border-radius: 16px;
      padding: 2.5rem 2rem;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.35);

      @media (min-width: 768px) {
        padding: 3.5rem 3rem;
      }
    }

    .walkin-eyebrow {
      display: inline-block;
      font-size: 0.8rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.15em;
      color: var(--accent-gold-primary);
      margin-bottom: 0.75rem;
    }

    .walkin-card__title {
      font-size: 1.85rem;
      color: var(--text-primary);
      line-height: 1.3;
      margin-bottom: 1rem;

      @media (min-width: 768px) {
        font-size: 2.3rem;
      }
    }

    .walkin-card__lead {
      font-size: 1.05rem;
      line-height: 1.65;
      color: var(--text-muted);
      max-width: 820px;
      margin-bottom: 2.5rem;
    }

    .walkin-steps {
      display: grid;
      grid-template-columns: 1fr;
      gap: 1.5rem;
      margin-bottom: 2.5rem;

      @media (min-width: 768px) {
        grid-template-columns: repeat(3, 1fr);
      }
    }

    .walkin-step {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
      background: rgba(12, 14, 18, 0.6);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 12px;
      padding: 1.5rem 1.25rem;
    }

    .walkin-step__icon {
      font-size: 1.75rem;
      line-height: 1;
      padding-top: 0.1rem;
    }

    .walkin-step__body strong {
      display: block;
      font-size: 0.95rem;
      color: var(--accent-gold-primary);
      margin-bottom: 0.4rem;
    }

    .walkin-step__body p {
      font-size: 0.85rem;
      line-height: 1.5;
      color: var(--text-muted);
      margin: 0;
    }

    .walkin-actions {
      display: flex;
      flex-wrap: wrap;
      gap: 1rem;
      padding-top: 1rem;
      border-top: 1px solid var(--border-secondary-subtle);
    }

    .btn-gold-action {
      display: inline-flex;
      align-items: center;
      gap: 0.6rem;
      background: linear-gradient(135deg, var(--accent-gold-primary) 0%, #b8860b 100%);
      color: #0c0e12;
      border: 1px solid var(--accent-gold-primary);
      font-family: var(--font-sans);
      font-size: 0.95rem;
      font-weight: 700;
      padding: 0.8rem 1.6rem;
      border-radius: 10px;
      text-decoration: none;
      transition: all 0.25s ease;

      &:hover {
        transform: translateY(-2px);
        box-shadow: 0 6px 20px rgba(212, 175, 55, 0.4);
      }
    }

    .btn-outline-action {
      display: inline-flex;
      align-items: center;
      gap: 0.6rem;
      background: var(--surface-secondary-walnut);
      color: var(--text-primary);
      border: 1px solid var(--border-secondary-subtle);
      font-family: var(--font-sans);
      font-size: 0.95rem;
      font-weight: 700;
      padding: 0.8rem 1.6rem;
      border-radius: 10px;
      text-decoration: none;
      transition: all 0.25s ease;

      &:hover {
        border-color: var(--accent-gold-primary);
        color: var(--accent-gold-primary);
        transform: translateY(-2px);
      }
    }
  `]
})
export class EventsPageComponent implements OnInit {
  private readonly dataService = inject(ResilientDataService);
  private readonly tableHubService = inject(TableHubService);
  private readonly seoMetaService = inject(SeoMetaService);
  private readonly jsonLdSchemaService = inject(JsonLdSchemaService);

  public readonly isLoading = signal<boolean>(true);
  public readonly currentEvent = signal<ClubEventModel | null>(null);
  public readonly selectedCategory = signal<string>('ALL');
  public readonly selectedTableForReservation = signal<GameTableModel | null>(null);

  public readonly allTables = computed(() => this.currentEvent()?.tables ?? []);

  public readonly filteredTables = computed(() => {
    const category = this.selectedCategory();
    const tables = this.allTables();
    if (category === 'ALL') {
      return tables;
    }
    return tables.filter((t) => t.category === category);
  });

  ngOnInit(): void {
    this.loadEventData();
    this.listenToSignalRUpdates();
  }

  private loadEventData(): void {
    this.dataService.getEvents().subscribe({
      next: (events) => {
        if (events && events.length > 0) {
          const ev = events[0];
          this.currentEvent.set(ev);
          this.seoMetaService.setMeta({
            title: ev.title,
            description: ev.tldrSummary,
            canonicalUrl: 'https://masabasikulubu.com/etkinlikler'
          });
          this.jsonLdSchemaService.setSchema([
            this.jsonLdSchemaService.buildOrganizationSchema(),
            this.jsonLdSchemaService.buildEventSchema(ev)
          ]);
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  private listenToSignalRUpdates(): void {
    this.tableHubService.seatUpdated$.subscribe((payload) => {
      const current = this.currentEvent();
      if (!current) return;

      const updatedTables = current.tables.map((table) => {
        if (table.id === payload.tableId) {
          return {
            ...table,
            currentPlayers: payload.currentPlayers,
            maxPlayers: payload.maxPlayers
          };
        }
        return table;
      });

      this.currentEvent.set({
        ...current,
        tables: updatedTables
      });
    });
  }

  public setCategory(cat: string): void {
    this.selectedCategory.set(cat);
  }

  public openReservationModal(table: GameTableModel): void {
    this.selectedTableForReservation.set(table);
  }

  public closeReservationModal(): void {
    this.selectedTableForReservation.set(null);
  }

  public handleReservationSuccess(result: ReservationResultModel): void {
    const current = this.currentEvent();
    if (!current) return;

    const updatedTables = current.tables.map((table) => {
      if (table.id === result.tableId) {
        return {
          ...table,
          currentPlayers: result.currentPlayers,
          maxPlayers: result.maxPlayers
        };
      }
      return table;
    });

    this.currentEvent.set({
      ...current,
      tables: updatedTables
    });
  }
}
