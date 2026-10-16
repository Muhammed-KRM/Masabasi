import { Component, ChangeDetectionStrategy, input, output, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GameTableModel, ReservationResultModel } from '../../../core/models/event-table.model';
import { ReservationService } from '../../../core/services/reservation.service';

@Component({
  selector: 'app-reservation-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="modal-backdrop" (click)="onBackdropClick($event)" role="dialog" aria-modal="true" aria-labelledby="modal-title">
      <div class="modal-card" (click)="$event.stopPropagation()">
        <header class="modal-card__header">
          <div>
            <span class="badge badge--emerald">Masa #{{ table().tableNumber }}</span>
            <h2 id="modal-title" class="modal-card__title">{{ table().gameTitle }}</h2>
          </div>
          <button type="button" class="modal-card__close" (click)="closed.emit()" aria-label="Kapat">×</button>
        </header>

        <div class="modal-card__body">
          @if (errorMessage()) {
            <div class="alert alert--error" role="alert">
              {{ errorMessage() }}
            </div>
          }

          @if (successResult()) {
            <div class="alert alert--success" role="alert">
              <h3>🎉 Harika! Yeriniz Ayrıldı</h3>
              <p>
                <strong>Masa #{{ successResult()!.tableNumber }} ({{ successResult()!.gameTitle }})</strong> için kaydınız başarıyla alındı.
                Cumartesi günü saat 11:00'de masada görüşmek üzere!
              </p>
              <button type="button" class="btn-gold modal-card__done-btn" (click)="closed.emit()">
                Tamam
              </button>
            </div>
          } @else {
            <form (ngSubmit)="submitForm()" #reservationForm="ngForm">
              <div class="form-group">
                <label for="playerName">Adınız ve Soyadınız *</label>
                <input
                  id="playerName"
                  name="playerName"
                  type="text"
                  class="form-control"
                  [(ngModel)]="playerName"
                  required
                  minlength="3"
                  placeholder="Örn: Burak Yılmaz"
                  [disabled]="isSubmitting()" />
              </div>

              <div class="form-group">
                <label for="contactHandle">İletişim (Telefon veya Instagram Kullanıcı Adı) *</label>
                <input
                  id="contactHandle"
                  name="contactHandle"
                  type="text"
                  class="form-control"
                  [(ngModel)]="contactHandle"
                  required
                  placeholder="Örn: @burak_boardgame veya 0532..."
                  [disabled]="isSubmitting()" />
                <small class="form-hint">Masa organizasyonu ve acil durum bilgilendirmesi için gereklidir.</small>
              </div>

              <div class="form-group">
                <label for="experienceLevel">Bu Oyun / Sistemdeki Tecrübeniz</label>
                <select
                  id="experienceLevel"
                  name="experienceLevel"
                  class="form-control"
                  [(ngModel)]="experienceLevel"
                  [disabled]="isSubmitting()">
                  <option value="Beginner">İlk Defa Oynayacağım (Kuralları Masada Dinleyeceğim)</option>
                  <option value="Intermediate">Temel Kuralları Biliyorum (Birkaç Kez Oynadım)</option>
                  <option value="Expert">Deneyimliyim (Akıcı Şekilde Oynayabilirim)</option>
                </select>
              </div>

              <div class="form-group">
                <label for="notes">Masa Yöneticisine (DM) Notunuz (İsteğe Bağlı)</label>
                <textarea
                  id="notes"
                  name="notes"
                  rows="2"
                  class="form-control"
                  [(ngModel)]="notes"
                  placeholder="Örn: 1. seviye büyücü oynatmak istiyorum..."
                  [disabled]="isSubmitting()"></textarea>
              </div>

              <div class="modal-card__footer">
                <button type="button" class="btn-outline-gold" (click)="closed.emit()" [disabled]="isSubmitting()">
                  Vazgeç
                </button>
                <button type="submit" class="btn-gold" [disabled]="isSubmitting() || !playerName || !contactHandle">
                  {{ isSubmitting() ? 'Kaydediliyor...' : 'Rezervasyonu Onayla' }}
                </button>
              </div>
            </form>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-backdrop {
      position: fixed;
      inset: 0;
      background-color: rgba(0, 0, 0, 0.75);
      backdrop-filter: blur(6px);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 1rem;
      animation: fadeIn 0.2s ease-out;
    }

    .modal-card {
      background: var(--bg-dominant-elevated);
      border: 1px solid var(--accent-gold-primary);
      box-shadow: var(--shadow-lg), var(--shadow-gold);
      border-radius: 14px;
      width: 100%;
      max-width: 520px;
      overflow: hidden;
      display: flex;
      flex-direction: column;
      animation: slideUp 0.25s ease-out;
    }

    .modal-card__header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      padding: 1.5rem 1.5rem 1rem;
      border-bottom: 1px solid var(--border-secondary-subtle);
    }

    .modal-card__title {
      font-size: 1.3rem;
      margin-top: 0.35rem;
    }

    .modal-card__close {
      background: transparent;
      border: none;
      color: var(--text-muted);
      font-size: 1.75rem;
      line-height: 1;
      cursor: pointer;
      padding: 0 0.5rem;
      transition: color 0.2s ease;
      &:hover { color: var(--accent-gold-primary); }
    }

    .modal-card__body {
      padding: 1.5rem;
    }

    .form-group {
      margin-bottom: 1.25rem;
    }

    label {
      display: block;
      font-size: 0.85rem;
      font-weight: 700;
      color: var(--text-primary);
      margin-bottom: 0.4rem;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem 0.9rem;
      background-color: var(--bg-dominant-primary);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 8px;
      color: var(--text-primary);
      font-family: var(--font-sans);
      font-size: 0.95rem;
      transition: border-color 0.2s ease, box-shadow 0.2s ease;
      box-sizing: border-box;

      &:focus {
        outline: none;
        border-color: var(--accent-gold-primary);
        box-shadow: 0 0 0 3px rgba(212, 175, 55, 0.2);
      }
    }

    .form-hint {
      display: block;
      color: var(--text-muted);
      font-size: 0.75rem;
      margin-top: 0.3rem;
    }

    .modal-card__footer {
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
      margin-top: 1.5rem;
      padding-top: 1rem;
      border-top: 1px solid var(--border-secondary-subtle);
    }

    .alert {
      padding: 1rem 1.2rem;
      border-radius: 8px;
      margin-bottom: 1.2rem;
      font-size: 0.95rem;

      &--error {
        background-color: var(--accent-crimson-bg);
        color: var(--accent-crimson-d20);
        border: 1px solid rgba(224, 90, 71, 0.4);
      }

      &--success {
        background-color: var(--accent-success-bg);
        color: var(--accent-success-seat);
        border: 1px solid rgba(52, 211, 153, 0.4);
        text-align: center;
        h3 { color: var(--accent-success-seat); margin-bottom: 0.5rem; }
        p { color: var(--text-primary); margin-bottom: 1rem; }
      }
    }

    .modal-card__done-btn {
      width: 100%;
    }

    @keyframes fadeIn {
      from { opacity: 0; }
      to { opacity: 1; }
    }

    @keyframes slideUp {
      from { transform: translateY(20px); opacity: 0; }
      to { transform: translateY(0); opacity: 1; }
    }
  `]
})
export class ReservationModalComponent {
  private readonly reservationService = inject(ReservationService);

  public readonly table = input.required<GameTableModel>();
  public readonly closed = output<void>();
  public readonly reservationSuccess = output<ReservationResultModel>();

  public playerName = '';
  public contactHandle = '';
  public experienceLevel = 'Beginner';
  public notes = '';

  public readonly isSubmitting = signal<boolean>(false);
  public readonly errorMessage = signal<string | null>(null);
  public readonly successResult = signal<ReservationResultModel | null>(null);

  public onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.closed.emit();
    }
  }

  public submitForm(): void {
    if (!this.playerName.trim() || !this.contactHandle.trim()) {
      this.errorMessage.set('Lütfen adınızı ve iletişim bilginizi giriniz.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.reservationService
      .makeReservation({
        gameTableId: this.table().id,
        playerName: this.playerName.trim(),
        contactHandle: this.contactHandle.trim(),
        experienceLevel: this.experienceLevel,
        notes: this.notes.trim() || null
      })
      .subscribe({
        next: (res) => {
          this.isSubmitting.set(false);
          this.successResult.set(res);
          this.reservationSuccess.emit(res);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          if (err.status === 409) {
            this.errorMessage.set(
              err.error?.detail || err.error?.message ||
              'Bu masada boş koltuk kalmadı veya bu iletişim bilgisiyle zaten bir kaydınız mevcut.'
            );
          } else if (err.status === 400) {
            this.errorMessage.set(err.error?.detail || 'Lütfen form alanlarını kontrol ediniz.');
          } else {
            this.errorMessage.set('Bağlantı sırasında bir hata oluştu. Lütfen tekrar deneyiniz.');
          }
        }
      });
  }
}
