import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';

export interface SeoMetaData {
  title: string;
  description: string;
  keywords?: string[];
  canonicalUrl?: string;
  ogType?: 'website' | 'article';
  ogImage?: string;
  robots?: string;
}

@Injectable({ providedIn: 'root' })
export class SeoMetaService {
  private readonly titleService = inject(Title);
  private readonly metaService = inject(Meta);
  private readonly doc = inject(DOCUMENT);
  private readonly platformId = inject(PLATFORM_ID);

  private readonly defaultTitle = 'Masabaşı Oyun Kulübü | İstanbul Kutu Oyunları & FRP Topluluğu';
  private readonly defaultDesc =
    "İstanbul'da her Cumartesi modern kutu oyunları (Catan, Secret Hitler, Avalon) ve D&D 5e masaüstü rol yapma oyunları (FRP) için buluşan sıcak ve yeni başlayan dostu topluluk.";
  private readonly siteUrl = 'https://masabasikulubu.com';
  private readonly defaultOgImage = 'https://masabasikulubu.com/images/masabasi-og-banner.webp';

  /**
   * Sayfanın SEO meta etiketlerini, OpenGraph ve Twitter kartlarını günceller.
   */
  public setMeta(data: Partial<SeoMetaData>): void {
    const title = data.title
      ? `${data.title} | Masabaşı Oyun Kulübü`
      : this.defaultTitle;
    const description = data.description || this.defaultDesc;
    const ogType = data.ogType || 'website';
    const ogImage = data.ogImage || this.defaultOgImage;
    const robots = data.robots || 'index, follow, max-snippet:-1, max-image-preview:large';
    const canonical = data.canonicalUrl || this.siteUrl;

    // Title
    this.titleService.setTitle(title);

    // Standart Meta Etiketleri
    this.metaService.updateTag({ name: 'description', content: description });
    this.metaService.updateTag({ name: 'robots', content: robots });
    if (data.keywords && data.keywords.length > 0) {
      this.metaService.updateTag({ name: 'keywords', content: data.keywords.join(', ') });
    }

    // OpenGraph
    this.metaService.updateTag({ property: 'og:site_name', content: 'Masabaşı Oyun Kulübü' });
    this.metaService.updateTag({ property: 'og:title', content: title });
    this.metaService.updateTag({ property: 'og:description', content: description });
    this.metaService.updateTag({ property: 'og:type', content: ogType });
    this.metaService.updateTag({ property: 'og:url', content: canonical });
    this.metaService.updateTag({ property: 'og:image', content: ogImage });
    this.metaService.updateTag({ property: 'og:locale', content: 'tr_TR' });

    // Twitter Card
    this.metaService.updateTag({ name: 'twitter:card', content: 'summary_large_image' });
    this.metaService.updateTag({ name: 'twitter:title', content: title });
    this.metaService.updateTag({ name: 'twitter:description', content: description });
    this.metaService.updateTag({ name: 'twitter:image', content: ogImage });

    // Canonical link
    this.updateCanonicalUrl(canonical);
  }

  private updateCanonicalUrl(url: string): void {
    try {
      let link: HTMLLinkElement | null = this.doc.querySelector("link[rel='canonical']");
      if (!link) {
        link = this.doc.createElement('link');
        link.setAttribute('rel', 'canonical');
        this.doc.head.appendChild(link);
      }
      link.setAttribute('href', url);
    } catch {
      // SSR ortamında link güvenli şekilde ele alınır
    }
  }
}
