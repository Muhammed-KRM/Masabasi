import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, ActivatedRoute } from '@angular/router';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';
import { of } from 'rxjs';

registerLocaleData(localeTr);
import { BlogClusterComponent } from './blog-cluster.component';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { ArticleSummaryModel } from '../../core/models/article.model';

describe('BlogClusterComponent', () => {
  let component: BlogClusterComponent;
  let fixture: ComponentFixture<BlogClusterComponent>;

  const mockArticles: ArticleSummaryModel[] = [
    {
      id: 'art-1',
      title: 'D&D 5e Nasıl Başlanır?',
      slug: 'dnd-5e-nasil-baslanir',
      topicCluster: 'frp-dnd',
      tldrSummary: 'D&D 5e başlangıç özeti.',
      coverImageUrl: null,
      estimatedReadTimeMinutes: 6,
      publishedAtUtc: '2026-10-06T12:00:00Z',
      viewCount: 100
    },
    {
      id: 'art-2',
      title: 'Secret Hitler Taktikleri',
      slug: 'secret-hitler-taktikleri',
      topicCluster: 'kutu-oyunlari',
      tldrSummary: 'Secret Hitler stratejileri.',
      coverImageUrl: null,
      estimatedReadTimeMinutes: 5,
      publishedAtUtc: '2026-10-09T12:00:00Z',
      viewCount: 150
    }
  ];

  beforeEach(async () => {
    const mockDataService = {
      getArticles: () => of(mockArticles)
    };

    await TestBed.configureTestingModule({
      imports: [BlogClusterComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              queryParamMap: {
                get: () => null
              }
            }
          }
        },
        { provide: ResilientDataService, useValue: mockDataService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(BlogClusterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load all articles', () => {
    expect(component).toBeTruthy();
    expect(component.allArticles().length).toBe(2);
    expect(component.filteredArticles().length).toBe(2);
  });

  it('should filter articles when activeCluster is changed', () => {
    component.setCluster('frp-dnd');
    expect(component.filteredArticles().length).toBe(1);
    expect(component.filteredArticles()[0].slug).toBe('dnd-5e-nasil-baslanir');

    component.setCluster('kutu-oyunlari');
    expect(component.filteredArticles().length).toBe(1);
    expect(component.filteredArticles()[0].slug).toBe('secret-hitler-taktikleri');

    component.setCluster('ALL');
    expect(component.filteredArticles().length).toBe(2);
  });

  it('should format cluster label correctly', () => {
    expect(component.getClusterLabel('frp-dnd')).toBe('FRP & D&D 5e');
    expect(component.getClusterLabel('kart-oyunlari')).toBe('Kart Oyunları (TCG)');
  });
});
