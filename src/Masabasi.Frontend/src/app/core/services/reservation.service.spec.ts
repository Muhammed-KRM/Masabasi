import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ReservationService } from './reservation.service';
import { CreateReservationModel, ReservationResultModel } from '../models/event-table.model';
import { environment } from '../../../environments/environment';

describe('ReservationService', () => {
  let service: ReservationService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ReservationService
      ]
    });

    service = TestBed.inject(ReservationService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('makeReservation should send POST to reservations endpoint', () => {
    const request: CreateReservationModel = {
      gameTableId: 'table-123',
      playerName: 'Can Demir',
      contactHandle: '@candemir',
      experienceLevel: 'Intermediate',
      notes: null
    };

    const mockResponse: ReservationResultModel = {
      reservationId: 'res-999',
      tableId: 'table-123',
      tableNumber: 1,
      gameTitle: 'Secret Hitler',
      currentPlayers: 6,
      maxPlayers: 10
    };

    let result: ReservationResultModel | undefined;
    service.makeReservation(request).subscribe((res) => {
      result = res;
    });

    const req = httpTesting.expectOne(`${environment.apiUrl}/api/v1/public/reservations`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush(mockResponse);

    expect(result).toBeDefined();
    expect(result!.currentPlayers).toBe(6);
  });
});
