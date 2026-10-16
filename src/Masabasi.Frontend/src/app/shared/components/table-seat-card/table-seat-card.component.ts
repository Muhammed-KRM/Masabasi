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
          <div
            class="table-card__progress-track"
            role="progressbar"
            [attr.aria-valuenow]="table().currentPlayers"
            [attr.aria-valuemin]="0"
            [attr.aria-valuemax]="table().maxPlayers">
            <div
              class="table-card__progress-fill"
              [class.table-card__progress-fill--full]="isFull()"
              [class.table-card__progress-fill--urgent]="remainingSeats() === 1"
              [style.width.%]="fillPercentage()"></div>
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
          <button
            type="button"
            class="btn-gold table-card__action-btn"
            (click)="reserveClicked.emit(table())">
            Masada Yerini Ayırt ({{ remainingSeats() }} Boş Koltuk)
          </button>
        } @else {
          <!-- Durum 2: API Kapalı / Statik Mod -> Meetup veya Instagram üzerinden sıfır hatayla yönlendir -->
          <a
            [href]="fallbackRegistrationUrl()"
            target="_blank"
            rel="noopener noreferrer"
            class="btn-gold table-card__action-btn">
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
      padding: 1.35rem;
      color: var(--text-primary);
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      gap: 1.15rem;
      transition: transform 0.25s ease, border-color 0.25s ease, box-shadow 0.25s ease;
      position: relative;
      overflow: hidden;
    }

    .table-card:hover {
      transform: translateY(-3px);
      border-color: var(--accent-gold-primary);
      box-shadow: var(--shadow-md);
    }

    .table-card--full {
      opacity: 0.8;
      border-color: rgba(184, 176, 164, 0.2);
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
      font-size: 0.85rem;
      border: 1px solid var(--border-secondary-subtle);
    }

    .table-card__badge {
      background-color: rgba(212, 175, 55, 0.12);
      color: var(--accent-gold-primary);
      font-size: 0.75rem;
      font-weight: 700;
      padding: 0.25rem 0.6rem;
      border-radius: 6px;
      border: 1px solid rgba(212, 175, 55, 0.25);
    }

    .table-card__beginner-tag {
      background-color: var(--accent-success-bg);
      color: var(--accent-success-seat);
      font-size: 0.75rem;
      font-weight: 700;
      padding: 0.25rem 0.6rem;
      border-radius: 6px;
      border: 1px solid rgba(52, 211, 153, 0.3);
    }

    .table-card__body {
      display: flex;
      flex-direction: column;
      gap: 0.6rem;
    }

    .table-card__title {
      font-family: var(--font-display);
      font-size: 1.2rem;
      margin: 0;
      color: var(--text-primary);
      line-height: 1.35;
    }

    .table-card__host {
      font-size: 0.9rem;
      color: var(--text-muted);
      margin: 0;
    }

    .table-card__host strong {
      color: var(--text-primary);
    }

    .table-card__capacity {
      margin-top: 0.35rem;
    }

    .table-card__capacity-labels {
      display: flex;
      justify-content: space-between;
      font-size: 0.85rem;
      color: var(--text-muted);
      margin-bottom: 0.4rem;
    }

    .table-card__progress-track {
      width: 100%;
      height: 8px;
      background-color: rgba(0, 0, 0, 0.3);
      border-radius: 9999px;
      overflow: hidden;
    }

    .table-card__progress-fill {
      height: 100%;
      background: linear-gradient(90deg, var(--surface-secondary-emerald) 0%, var(--accent-gold-primary) 100%);
      transition: width 0.4s ease;
      border-radius: 9999px;
    }

    .table-card__progress-fill--urgent {
      background: linear-gradient(90deg, var(--accent-gold-primary) 0%, var(--accent-crimson-d20) 100%);
    }

    .table-card__progress-fill--full {
      background: #71717A;
    }

    .table-card__footer {
      margin-top: auto;
    }

    .table-card__action-btn {
      width: 100%;
    }

    .text-crimson {
      color: var(--accent-crimson-d20) !important;
      font-weight: 800;
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
