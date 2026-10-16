import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { ResilientDataService } from '../../../core/services/resilient-data.service';

@Component({
  selector: 'app-active-mode-badge',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div
      class="mode-badge"
      [class.mode-badge--live]="dataService.activeMode() === 'LIVE_API'"
      [class.mode-badge--static]="dataService.activeMode() === 'STATIC_FALLBACK'"
      [attr.title]="tooltipText()">
      <span class="mode-badge__dot"></span>
      <span class="mode-badge__text">
        {{ dataService.activeMode() === 'LIVE_API' ? 'Canlı Senkronizasyon' : 'Bakırköy / İstanbul' }}
      </span>
    </div>
  `,
  styles: [`
    .mode-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.3rem 0.75rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 700;
      letter-spacing: 0.03em;
      border: 1px solid transparent;
      user-select: none;
      transition: all 0.3s ease;
    }

    .mode-badge--live {
      background-color: var(--accent-success-bg);
      color: var(--accent-success-seat);
      border-color: rgba(52, 211, 153, 0.3);
    }

    .mode-badge--live .mode-badge__dot {
      background-color: var(--accent-success-seat);
      box-shadow: 0 0 8px var(--accent-success-seat);
    }

    .mode-badge--static {
      background-color: var(--accent-crimson-bg);
      color: var(--accent-gold-primary);
      border-color: rgba(212, 175, 55, 0.3);
    }

    .mode-badge--static .mode-badge__dot {
      background-color: var(--accent-gold-primary);
      box-shadow: 0 0 8px var(--accent-gold-primary);
    }

    .mode-badge__dot {
      width: 7px;
      height: 7px;
      border-radius: 50%;
      animation: pulseDot 2s infinite ease-in-out;
    }

    @keyframes pulseDot {
      0%, 100% { transform: scale(1); opacity: 1; }
      50% { transform: scale(1.3); opacity: 0.7; }
    }
  `]
})
export class ActiveModeBadgeComponent {
  public readonly dataService = inject(ResilientDataService);

  public tooltipText(): string {
    return this.dataService.activeMode() === 'LIVE_API'
      ? '.NET 10 API ve SignalR ile canlı masa ve koltuk senkronizasyonu aktif.'
      : 'API çevrimdışı; site statik JSON snapshot ile kesintisiz çalışıyor. Kayıtlar Meetup/Instagram yönlendirmesiyle alınır.';
  }
}
