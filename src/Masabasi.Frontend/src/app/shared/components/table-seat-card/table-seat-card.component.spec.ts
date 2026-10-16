import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TableSeatCardComponent } from './table-seat-card.component';
import { GameTableModel } from '../../../core/models/event-table.model';
import { ResilientDataService } from '../../../core/services/resilient-data.service';

describe('TableSeatCardComponent', () => {
  let component: TableSeatCardComponent;
  let fixture: ComponentFixture<TableSeatCardComponent>;
  let dataService: ResilientDataService;

  const mockTable: GameTableModel = {
    id: 'table-1',
    tableNumber: 2,
    gameTitle: 'Dungeons & Dragons 5e: Zindanların Çağrısı',
    category: 'FRP_DnD',
    hostOrDmName: 'DM Kerem',
    maxPlayers: 5,
    currentPlayers: 3,
    isBeginnerFriendly: true
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TableSeatCardComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ResilientDataService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TableSeatCardComponent);
    component = fixture.componentInstance;
    dataService = TestBed.inject(ResilientDataService);

    fixture.componentRef.setInput('table', mockTable);
    fixture.detectChanges();
  });

  it('should render table details accurately', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.table-card__number')?.textContent).toContain('Masa #2');
    expect(el.querySelector('.table-card__title')?.textContent).toContain('Dungeons & Dragons 5e');
    expect(el.querySelector('.table-card__host')?.textContent).toContain('DM Kerem');
    expect(el.querySelector('.table-card__beginner-tag')?.textContent).toContain('Yeni Başlayan Dostu');
  });

  it('should calculate remaining seats and fill percentage', () => {
    expect(component.remainingSeats()).toBe(2);
    expect(component.fillPercentage()).toBe(60);
    expect(component.isFull()).toBe(false);
  });

  it('should highlight urgent seat when remainingSeats is 1', () => {
    fixture.componentRef.setInput('table', { ...mockTable, currentPlayers: 4 });
    fixture.detectChanges();

    expect(component.remainingSeats()).toBe(1);
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('(Son 1 Koltuk!)');
    expect(el.querySelector('.text-crimson')).toBeTruthy();
  });

  it('should show disabled state when table is completely full', () => {
    fixture.componentRef.setInput('table', { ...mockTable, currentPlayers: 5 });
    fixture.detectChanges();

    expect(component.isFull()).toBe(true);
    const el = fixture.nativeElement as HTMLElement;
    const btn = el.querySelector('.btn-disabled') as HTMLButtonElement;
    expect(btn).toBeTruthy();
    expect(btn.disabled).toBe(true);
    expect(btn.textContent).toContain('Masa Kontenjanı Doldu');
  });

  it('should emit reserveClicked in LIVE_API mode', () => {
    dataService.activeMode.set('LIVE_API');
    fixture.detectChanges();

    let emitted: GameTableModel | undefined;
    component.reserveClicked.subscribe((t) => {
      emitted = t;
    });

    const el = fixture.nativeElement as HTMLElement;
    const actionBtn = el.querySelector('.btn-gold') as HTMLButtonElement;
    expect(actionBtn).toBeTruthy();
    actionBtn.click();

    expect(emitted).toBeDefined();
    expect(emitted!.id).toBe('table-1');
  });

  it('should show fallback link in STATIC_FALLBACK mode', () => {
    dataService.activeMode.set('STATIC_FALLBACK');
    fixture.detectChanges();

    const el = fixture.nativeElement as HTMLElement;
    const fallbackLink = el.querySelector('a.btn-gold') as HTMLAnchorElement;
    expect(fallbackLink).toBeTruthy();
    expect(fallbackLink.textContent).toContain('Meetup / Instagram ile Masaya Katıl');
  });
});
