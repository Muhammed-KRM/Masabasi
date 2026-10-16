import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { GameDetailComponent } from './game-detail.component';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { BoardGameModel } from '../../core/models/board-game.model';

describe('GameDetailComponent', () => {
  let component: GameDetailComponent;
  let fixture: ComponentFixture<GameDetailComponent>;

  const mockGame: BoardGameModel = {
    id: 'g-1',
    title: 'Catan',
    slug: 'catan',
    category: 'BoardGame',
    minPlayers: 3,
    maxPlayers: 4,
    recommendedPlayerCount: '4 Oyuncu',
    playTimeMinutes: 75,
    complexityRating: 2.3,
    tldrSummary: 'Ada ticareti ve kaynak yönetimi.',
    rulesMarkdown: '### Catan Kuralları',
    isInClubInventory: true,
    yearPublished: 1995
  };

  beforeEach(async () => {
    const mockDataService = {
      getGameBySlug: (slug: string) => of(slug === 'catan' ? mockGame : null)
    };

    await TestBed.configureTestingModule({
      imports: [GameDetailComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => (key === 'slug' ? 'catan' : null)
              }
            }
          }
        },
        { provide: ResilientDataService, useValue: mockDataService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GameDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load game details from slug', () => {
    expect(component).toBeTruthy();
    expect(component.game()).toEqual(mockGame);
    expect(component.isLoading()).toBe(false);

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.game-header__title')?.textContent).toContain('Catan');
    expect(compiled.querySelector('.rules-pre')?.textContent).toContain('### Catan Kuralları');
  });

  it('should format category label correctly', () => {
    expect(component.getCategoryLabel('BoardGame')).toBe('Kutu Oyunu');
    expect(component.getCategoryLabel('FRP_DnD')).toBe('FRP / D&D 5e');
  });
});
