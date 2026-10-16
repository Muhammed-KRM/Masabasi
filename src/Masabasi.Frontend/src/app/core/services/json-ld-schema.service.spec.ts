import { TestBed } from '@angular/core/testing';
import { DOCUMENT } from '@angular/common';
import { JsonLdSchemaService } from './json-ld-schema.service';
import { ClubEventModel } from '../models/event-table.model';
import { BoardGameModel } from '../models/board-game.model';
import { ArticleModel } from '../models/article.model';

describe('JsonLdSchemaService', () => {
  let service: JsonLdSchemaService;
  let doc: Document;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [JsonLdSchemaService]
    });

    service = TestBed.inject(JsonLdSchemaService);
    doc = TestBed.inject(DOCUMENT);
  });

  afterEach(() => {
    service.removeSchema('test-schema');
    service.removeSchema('masabasi-jsonld');
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should inject script tag with application/ld+json into document head', () => {
    const dummy = { '@context': 'https://schema.org', '@type': 'Thing', name: 'Test' };
    service.setSchema(dummy, 'test-schema');

    const script = doc.getElementById('test-schema') as HTMLScriptElement;
    expect(script).toBeTruthy();
    expect(script.type).toBe('application/ld+json');
    expect(JSON.parse(script.text)).toEqual(dummy);
  });

  it('should build valid Organization schema', () => {
    const org = service.buildOrganizationSchema();
    expect(org['@type']).toBe('SportsClub');
    expect(org['name']).toBe('Masabaşı Oyun Kulübü');
    expect(org['url']).toBe('https://masabasikulubu.com');
  });

  it('should build valid Event schema with location, offers and subEvents', () => {
    const mockEvent: ClubEventModel = {
      id: 'ev-1',
      title: 'Cumartesi Buluşması',
      slug: 'cumartesi-bulusmasi',
      tldrSummary: 'Haftalık masaüstü oyun buluşması.',
      startDateUtc: '2026-10-17T11:00:00Z',
      endDateUtc: '2026-10-17T18:00:00Z',
      venueName: 'Masabaşı Kadıköy',
      venueAddress: 'Kadıköy / İstanbul',
      meetupEventUrl: 'https://meetup.com/event',
      externalRegistrationUrl: null,
      isActive: true,
      tables: [
        {
          id: 'tab-1',
          tableNumber: 1,
          gameTitle: 'Secret Hitler',
          category: 'SocialDeduction',
          hostOrDmName: 'Masabaşı Ekibi',
          maxPlayers: 10,
          currentPlayers: 8,
          isBeginnerFriendly: true
        }
      ]
    };

    const eventSchema = service.buildEventSchema(mockEvent);
    expect(eventSchema['@type']).toBe('SocialEvent');
    expect(eventSchema['name']).toBe('Cumartesi Buluşması');
    expect(Array.isArray(eventSchema['subEvent'])).toBe(true);
    expect((eventSchema['subEvent'] as Array<unknown>).length).toBe(1);
  });

  it('should build valid Game schema', () => {
    const mockGame: BoardGameModel = {
      id: 'g-1',
      title: 'Catan',
      slug: 'catan',
      category: 'BoardGame',
      minPlayers: 3,
      maxPlayers: 4,
      playTimeMinutes: 75,
      complexityRating: 2.3,
      tldrSummary: 'Catan ada ticareti.',
      rulesMarkdown: 'Kurallar...',
      isInClubInventory: true
    };

    const gameSchema = service.buildGameSchema(mockGame);
    expect(gameSchema['@type']).toBe('Game');
    expect(gameSchema['name']).toBe('Catan');
    expect(gameSchema['timeRequired']).toBe('PT75M');
  });

  it('should build valid Article schema', () => {
    const mockArticle: ArticleModel = {
      id: 'art-1',
      title: 'D&D 5e Rehberi',
      slug: 'dnd-5e-rehberi',
      topicCluster: 'frp-dnd',
      tldrSummary: 'D&D 5e başlangıç özeti.',
      metaDescription: 'SEO açıklaması.',
      contentHtml: '<p>İçerik</p>',
      contentMarkdown: '## İçerik',
      authorName: 'Efe DM',
      estimatedReadTimeMinutes: 5,
      publishedAtUtc: '2026-10-10T12:00:00Z',
      viewCount: 10
    };

    const articleSchema = service.buildArticleSchema(mockArticle);
    expect(articleSchema['@type']).toBe('Article');
    expect(articleSchema['headline']).toBe('D&D 5e Rehberi');
    expect((articleSchema['author'] as Record<string, unknown>)['name']).toBe('Efe DM');
  });

  it('should build valid FAQ schema', () => {
    const faqs = [
      { question: 'Zar getirmeli miyim?', answer: 'Kulüp temin eder.' }
    ];

    const faqSchema = service.buildFaqSchema(faqs);
    expect(faqSchema['@type']).toBe('FAQPage');
    expect(Array.isArray(faqSchema['mainEntity'])).toBe(true);
  });
});
