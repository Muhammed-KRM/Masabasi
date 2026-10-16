import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';
import { of, Subject } from 'rxjs';
import { EventsPageComponent } from './events-page.component';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { TableHubService } from '../../core/services/table-hub.service';
import { ClubEventModel, TableSeatUpdatedPayload } from '../../core/models/event-table.model';

registerLocaleData(localeTr);

describe('EventsPageComponent', () => {
  let component: EventsPageComponent;
  let fixture: ComponentFixture<EventsPageComponent>;
  let dataService: ResilientDataService;
  let seatUpdatedSubject: Subject<TableSeatUpdatedPayload>;

  const mockEvents: ClubEventModel[] = [
    {
      id: 'event-1',
      title: 'Cumartesi Buluşması: Kutu Oyunları',
      slug: 'cumartesi-bulusmasi',
      tldrSummary: 'Bu Cumartesi kutu oyunları buluşması.',
      startDateUtc: '2026-10-17T11:00:00Z',
      endDateUtc: '2026-10-17T18:00:00Z',
      venueName: 'Kadıköy Kulüp Evi',
      venueAddress: 'Moda Cad. No:42 Kadıköy',
      meetupEventUrl: 'https://meetup.com',
      externalRegistrationUrl: 'https://instagram.com',
      tables: [
        {
          id: 'table-1',
          tableNumber: 1,
          gameTitle: 'Secret Hitler',
          category: 'SocialDeduction',
          hostOrDmName: 'Efe',
          maxPlayers: 10,
          currentPlayers: 7,
          isBeginnerFriendly: true
        },
        {
          id: 'table-2',
          tableNumber: 2,
          gameTitle: 'D&D 5e',
          category: 'FRP_DnD',
          hostOrDmName: 'DM Kerem',
          maxPlayers: 5,
          currentPlayers: 4,
          isBeginnerFriendly: true
        }
      ]
    }
  ];

  beforeEach(async () => {
    seatUpdatedSubject = new Subject<TableSeatUpdatedPayload>();

    const mockTableHub = {
      seatUpdated$: seatUpdatedSubject.asObservable(),
      joinTableGroup: vi.fn(),
      leaveTableGroup: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [EventsPageComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ResilientDataService,
        { provide: TableHubService, useValue: mockTableHub }
      ]
    }).compileComponents();

    dataService = TestBed.inject(ResilientDataService);
    vi.spyOn(dataService, 'getEvents').mockReturnValue(of(mockEvents));

    fixture = TestBed.createComponent(EventsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load event data on init', () => {
    expect(component.isLoading()).toBe(false);
    expect(component.currentEvent()).toEqual(mockEvents[0]);
    expect(component.allTables().length).toBe(2);
  });

  it('should filter tables by category', () => {
    expect(component.filteredTables().length).toBe(2);

    component.setCategory('FRP_DnD');
    expect(component.filteredTables().length).toBe(1);
    expect(component.filteredTables()[0].gameTitle).toBe('D&D 5e');

    component.setCategory('SocialDeduction');
    expect(component.filteredTables().length).toBe(1);
    expect(component.filteredTables()[0].gameTitle).toBe('Secret Hitler');

    component.setCategory('ALL');
    expect(component.filteredTables().length).toBe(2);
  });

  it('should open and close reservation modal', () => {
    expect(component.selectedTableForReservation()).toBeNull();

    component.openReservationModal(mockEvents[0].tables[0]);
    expect(component.selectedTableForReservation()).toEqual(mockEvents[0].tables[0]);

    component.closeReservationModal();
    expect(component.selectedTableForReservation()).toBeNull();
  });

  it('should update table player count on reservation success', () => {
    component.handleReservationSuccess({
      reservationId: 'res-123',
      tableId: 'table-1',
      tableNumber: 1,
      gameTitle: 'Secret Hitler',
      currentPlayers: 8,
      maxPlayers: 10
    });

    const updatedTable = component.allTables().find((t) => t.id === 'table-1');
    expect(updatedTable?.currentPlayers).toBe(8);
  });

  it('should update table player count on real-time SignalR seatUpdated event', () => {
    seatUpdatedSubject.next({
      tableId: 'table-2',
      currentPlayers: 5,
      maxPlayers: 5
    });

    const updatedTable = component.allTables().find((t) => t.id === 'table-2');
    expect(updatedTable?.currentPlayers).toBe(5);
  });
});
