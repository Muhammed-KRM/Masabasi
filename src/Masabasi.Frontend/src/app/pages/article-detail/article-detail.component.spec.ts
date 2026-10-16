import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, ActivatedRoute } from '@angular/router';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';
import { of } from 'rxjs';

registerLocaleData(localeTr);
import { ArticleDetailComponent } from './article-detail.component';
import { ResilientDataService } from '../../core/services/resilient-data.service';
import { ArticleModel } from '../../core/models/article.model';

describe('ArticleDetailComponent', () => {
  let component: ArticleDetailComponent;
  let fixture: ComponentFixture<ArticleDetailComponent>;

  const mockArticle: ArticleModel = {
    id: 'art-1',
    title: 'D&D 5e Nasıl Başlanır?',
    slug: 'dnd-5e-nasil-baslanir',
    topicCluster: 'frp-dnd',
    tldrSummary: 'D&D 5e başlangıç özeti.',
    metaDescription: 'SEO açıklaması.',
    contentHtml: '<p>Masaüstü FRP dünyasına hoş geldiniz.</p>',
    contentMarkdown: '## Giriş',
    authorName: 'Efe (Dungeon Master)',
    faqJson: '[{"question":"Zar gerekir mi?","answer":"Kulüp temin eder."}]',
    coverImageUrl: null,
    estimatedReadTimeMinutes: 6,
    publishedAtUtc: '2026-10-06T12:00:00Z',
    viewCount: 100
  };

  beforeEach(async () => {
    const mockDataService = {
      getArticleBySlug: (slug: string) =>
        of(slug === 'dnd-5e-nasil-baslanir' ? mockArticle : null)
    };

    await TestBed.configureTestingModule({
      imports: [ArticleDetailComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) =>
                  key === 'slug' ? 'dnd-5e-nasil-baslanir' : 'frp-dnd'
              }
            }
          }
        },
        { provide: ResilientDataService, useValue: mockDataService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ArticleDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load article detail from slug', () => {
    expect(component).toBeTruthy();
    expect(component.article()).toEqual(mockArticle);
    expect(component.isLoading()).toBe(false);

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.article-header__title')?.textContent).toContain(
      'D&D 5e Nasıl Başlanır?'
    );
    expect(compiled.querySelector('.author')?.textContent).toContain('Efe (Dungeon Master)');
  });

  it('should parse faqJson and set faqs signal', () => {
    expect(component.faqs().length).toBe(1);
    expect(component.faqs()[0].question).toBe('Zar gerekir mi?');
  });

  it('should format cluster label correctly', () => {
    expect(component.getClusterLabel('frp-dnd')).toBe('FRP & D&D 5e');
  });
});
