import { Component, ChangeDetectionStrategy, input } from '@angular/core';

@Component({
  selector: 'app-tldr-ai-box',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <aside class="tldr-box" role="complementary" aria-label="Hızlı Özet">
      <div class="tldr-box__header">
        <span class="tldr-box__icon" aria-hidden="true">📜</span>
        <span class="tldr-box__badge">GEO & AI Hızlı Özet (TL;DR)</span>
      </div>
      <p class="tldr-box__text">
        {{ summary() }}
      </p>
    </aside>
  `,
  styles: [`
    .tldr-box {
      background: linear-gradient(135deg, var(--surface-secondary-walnut) 0%, rgba(30, 58, 52, 0.4) 100%);
      border: 1px solid var(--border-card);
      border-left: 4px solid var(--accent-gold-primary);
      border-radius: 10px;
      padding: 1.15rem 1.4rem;
      margin: 1.5rem 0;
      box-shadow: var(--shadow-sm);
      transition: border-color 0.3s ease;
    }

    .tldr-box:hover {
      border-color: var(--accent-gold-primary);
    }

    .tldr-box__header {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 0.6rem;
    }

    .tldr-box__icon {
      font-size: 1.1rem;
    }

    .tldr-box__badge {
      font-family: var(--font-display);
      font-size: 0.75rem;
      font-weight: 700;
      color: var(--accent-gold-primary);
      letter-spacing: 0.08em;
      text-transform: uppercase;
    }

    .tldr-box__text {
      color: var(--text-primary);
      font-size: 0.95rem;
      line-height: 1.6;
      font-style: italic;
      margin: 0;
    }
  `]
})
export class TldrAiBoxComponent {
  public readonly summary = input.required<string>();
}
