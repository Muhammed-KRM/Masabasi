import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { GameCatalogComponent } from './game-catalog.component';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { BoardGameModel } from '../../core/models/board-game.model';

describe('GameCatalogComponent', () => {
  let component: GameCatalogComponent;
  let fixture: ComponentFixture<GameCatalogComponent>;

  const mockGames: BoardGameModel[] = [
    {
      id: 'g-1',
      title: 'Secret Hitler',
      slug: 'secret-hitler',
      category: 'SocialDeduction',
      minPlayers: 5,
      maxPlayers: 10,
      playTimeMinutes: 45,
      complexityRating: 2.1,
      tldrSummary: 'Sosyal çıkarım ve blöf.',
      rulesMarkdown: 'Kurallar...',
      isInClubInventory: true
    },
    {
      id: 'g-2',
      title: 'Catan',
      slug: 'catan',
      category: 'BoardGame',
      minPlayers: 3,
      maxPlayers: 4,
      playTimeMinutes: 75,
      complexityRating: 2.3,
      tldrSummary: 'Ada ticareti ve kaynak yönetimi.',
      rulesMarkdown: 'Kurallar...',
      isInClubInventory: true
    }
  ];

  beforeEach(async () => {
    const mockDataService = {
      getGames: () => of(mockGames)
    };

    await TestBed.configureTestingModule({
      imports: [GameCatalogComponent],
      providers: [
        provideRouter([]),
        { provide: ResilientDataService, useValue: mockDataService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GameCatalogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load games', () => {
    expect(component).toBeTruthy();
    expect(component.allGames().length).toBe(2);
    expect(component.filteredGames().length).toBe(2);
  });

  it('should filter games by category', () => {
    component.selectedCategory.set('BoardGame');
    expect(component.filteredGames().length).toBe(1);
    expect(component.filteredGames()[0].title).toBe('Catan');
  });

  it('should filter games by search query', () => {
    component.searchQuery.set('Hitler');
    expect(component.filteredGames().length).toBe(1);
    expect(component.filteredGames()[0].title).toBe('Secret Hitler');
  });

  it('should reset filters when resetFilters is called', () => {
    component.selectedCategory.set('BoardGame');
    component.searchQuery.set('test');
    component.resetFilters();

    expect(component.selectedCategory()).toBe('ALL');
    expect(component.searchQuery()).toBe('');
    expect(component.filteredGames().length).toBe(2);
  });
});
