import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { ReservationModalComponent } from './reservation-modal.component';
import { ReservationService } from '../../../core/services/reservation.service';
import { GameTableModel, ReservationResultModel } from '../../../core/models/event-table.model';

describe('ReservationModalComponent', () => {
  let component: ReservationModalComponent;
  let fixture: ComponentFixture<ReservationModalComponent>;
  let reservationService: ReservationService;

  const mockTable: GameTableModel = {
    id: 'table-123',
    tableNumber: 3,
    gameTitle: 'Secret Hitler',
    category: 'SocialDeduction',
    hostOrDmName: 'Moderatör',
    maxPlayers: 10,
    currentPlayers: 7,
    isBeginnerFriendly: true
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReservationModalComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ReservationService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ReservationModalComponent);
    component = fixture.componentInstance;
    reservationService = TestBed.inject(ReservationService);

    fixture.componentRef.setInput('table', mockTable);
    fixture.detectChanges();
  });

  it('should render table info and title', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.badge')?.textContent).toContain('Masa #3');
    expect(el.querySelector('#modal-title')?.textContent).toContain('Secret Hitler');
  });

  it('should emit closed when close button is clicked', () => {
    let closedEmitted = false;
    component.closed.subscribe(() => {
      closedEmitted = true;
    });

    const closeBtn = fixture.nativeElement.querySelector('.modal-card__close') as HTMLButtonElement;
    closeBtn.click();

    expect(closedEmitted).toBe(true);
  });

  it('should show error if name or contact is empty on submit', () => {
    component.playerName = '';
    component.contactHandle = '';
    component.submitForm();

    expect(component.errorMessage()).toContain('Lütfen adınızı ve iletişim bilginizi giriniz.');
  });

  it('should display success state when reservation completes', () => {
    const mockSuccess: ReservationResultModel = {
      reservationId: 'res-1',
      tableId: 'table-123',
      tableNumber: 3,
      gameTitle: 'Secret Hitler',
      currentPlayers: 8,
      maxPlayers: 10
    };

    vi.spyOn(reservationService, 'makeReservation').mockReturnValue(of(mockSuccess));

    component.playerName = 'Efe Yılmaz';
    component.contactHandle = '@efeyilmaz';
    component.submitForm();

    fixture.detectChanges();

    expect(component.successResult()).toEqual(mockSuccess);
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.alert--success')).toBeTruthy();
    expect(el.textContent).toContain('Yeriniz Ayrıldı');
  });

  it('should handle 409 conflict error properly', () => {
    vi.spyOn(reservationService, 'makeReservation').mockReturnValue(
      throwError(() => ({
        status: 409,
        error: { detail: 'Masa kontenjanı dolmuştur.' }
      }))
    );

    component.playerName = 'Efe Yılmaz';
    component.contactHandle = '@efeyilmaz';
    component.submitForm();

    fixture.detectChanges();

    expect(component.errorMessage()).toContain('Masa kontenjanı dolmuştur.');
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.alert--error')).toBeTruthy();
  });
});
