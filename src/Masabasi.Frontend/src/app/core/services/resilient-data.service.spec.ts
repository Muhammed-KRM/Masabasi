import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ResilientDataService } from './resilient-data.service';
import { ClubEventModel } from '../models/event-table.model';
import { BoardGameModel } from '../models/board-game.model';
import { environment } from '../../../environments/environment';

describe('ResilientDataService', () => {
  let service: ResilientDataService;
  let httpTesting: HttpTestingController;

  const mockEvents: ClubEventModel[] = [
    {
      id: 'event-1',
      title: 'Cumartesi Buluşması',
      slug: 'cumartesi-bulusmasi',
      tldrSummary: 'Kısa özet açıklama.',
      startDateUtc: '2026-10-17T11:00:00Z',
      endDateUtc: '2026-10-17T18:00:00Z',
      venueName: 'Kadıköy',
      venueAddress: 'İstanbul',
      meetupEventUrl: null,
      externalRegistrationUrl: null,
      tables: []
    }
  ];

  const mockGames: BoardGameModel[] = [
    {
      id: 'game-1',
      title: 'Catan',
      slug: 'catan',
      category: 'BoardGame',
      minPlayers: 3,
      maxPlayers: 4,
      playTimeMinutes: 75,
      complexityRating: 2.3,
      tldrSummary: 'Özet',
      rulesMarkdown: 'Kurallar',
      isInClubInventory: true
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ResilientDataService
      ]
    });

    service = TestBed.inject(ResilientDataService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created and default to LIVE_API mode', () => {
    expect(service).toBeTruthy();
    expect(service.activeMode()).toBe('LIVE_API');
  });

  it('getEvents should return events from live API when successful', () => {
    let result: ClubEventModel[] = [];

    service.getEvents().subscribe((events) => {
      result = events;
    });

    const req = httpTesting.expectOne(`${environment.apiUrl}/api/v1/public/events`);
    expect(req.request.method).toBe('GET');
    req.flush(mockEvents);

    expect(result.length).toBe(1);
    expect(result[0].title).toBe('Cumartesi Buluşması');
    expect(service.activeMode()).toBe('LIVE_API');
  });

  it('getEvents should fallback to static snapshot when live API errors', () => {
    let result: ClubEventModel[] = [];

    service.getEvents().subscribe((events) => {
      result = events;
    });

    // 1. Live API çağrısı hata ile döner (500 Internal Server Error)
    const apiReq = httpTesting.expectOne(`${environment.apiUrl}/api/v1/public/events`);
    apiReq.flush('Sunucu hatası', { status: 500, statusText: 'Internal Server Error' });

    // 2. Servis otomatik olarak statik snapshot dosyasına fallback yapar
    const fallbackReq = httpTesting.expectOne('/data/events.snapshot.json');
    expect(fallbackReq.request.method).toBe('GET');
    fallbackReq.flush(mockEvents);

    expect(result.length).toBe(1);
    expect(service.activeMode()).toBe('STATIC_FALLBACK');
  });

  it('getGames should return board games and fallback on error', () => {
    let result: BoardGameModel[] = [];

    service.getGames().subscribe((games) => {
      result = games;
    });

    // API hatası simüle et
    const apiReq = httpTesting.expectOne(`${environment.apiUrl}/api/v1/public/games`);
    apiReq.flush('Network error', { status: 503, statusText: 'Service Unavailable' });

    // Statik snapshot'a geçiş
    const fallbackReq = httpTesting.expectOne('/data/games.snapshot.json');
    fallbackReq.flush(mockGames);

    expect(result.length).toBe(1);
    expect(result[0].title).toBe('Catan');
    expect(service.activeMode()).toBe('STATIC_FALLBACK');
  });
});
