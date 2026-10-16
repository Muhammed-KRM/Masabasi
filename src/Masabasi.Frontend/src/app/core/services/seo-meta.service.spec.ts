import { TestBed } from '@angular/core/testing';
import { Title, Meta } from '@angular/platform-browser';
import { DOCUMENT } from '@angular/common';
import { SeoMetaService } from './seo-meta.service';

describe('SeoMetaService', () => {
  let service: SeoMetaService;
  let titleService: Title;
  let metaService: Meta;
  let doc: Document;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SeoMetaService]
    });

    service = TestBed.inject(SeoMetaService);
    titleService = TestBed.inject(Title);
    metaService = TestBed.inject(Meta);
    doc = TestBed.inject(DOCUMENT);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should set formatted title with site suffix', () => {
    service.setMeta({ title: 'Catan Rehberi' });
    expect(titleService.getTitle()).toBe('Catan Rehberi | Masabaşı Oyun Kulübü');
  });

  it('should set default title when no title is provided', () => {
    service.setMeta({});
    expect(titleService.getTitle()).toContain('Masabaşı Oyun Kulübü');
  });

  it('should update description, robots, and OpenGraph tags', () => {
    service.setMeta({
      description: 'Harika bir kutu oyunu kılavuzu.',
      ogType: 'article',
      canonicalUrl: 'https://masabasikulubu.com/test'
    });

    const descTag = metaService.getTag('name="description"');
    expect(descTag?.content).toBe('Harika bir kutu oyunu kılavuzu.');

    const ogTypeTag = metaService.getTag('property="og:type"');
    expect(ogTypeTag?.content).toBe('article');

    const ogUrlTag = metaService.getTag('property="og:url"');
    expect(ogUrlTag?.content).toBe('https://masabasikulubu.com/test');
  });

  it('should set canonical link in document head', () => {
    service.setMeta({ canonicalUrl: 'https://masabasikulubu.com/canonical-test' });
    const link = doc.querySelector("link[rel='canonical']");
    expect(link).toBeTruthy();
    expect(link?.getAttribute('href')).toBe('https://masabasikulubu.com/canonical-test');
  });
});
