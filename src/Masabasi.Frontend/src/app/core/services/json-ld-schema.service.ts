import { Injectable, inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { ClubEventModel } from '../models/event-table.model';
import { BoardGameModel } from '../models/board-game.model';
import { ArticleModel } from '../models/article.model';

@Injectable({ providedIn: 'root' })
export class JsonLdSchemaService {
  private readonly doc = inject(DOCUMENT);

  /**
   * HTML <head> içine JSON-LD script etiketini enjekte eder veya günceller.
   */
  public setSchema(schema: Record<string, unknown> | Array<Record<string, unknown>>, id = 'masabasi-jsonld'): void {
    this.removeSchema(id);

    try {
      const script = this.doc.createElement('script');
      script.id = id;
      script.type = 'application/ld+json';
      script.text = JSON.stringify(schema, null, 2);
      this.doc.head.appendChild(script);
    } catch {
      // Güvenli hata yakalama
    }
  }

  /**
   * Belirtilen ID'ye sahip schema etiketini kaldırır.
   */
  public removeSchema(id = 'masabasi-jsonld'): void {
    try {
      const existing = this.doc.getElementById(id);
      if (existing) {
        existing.remove();
      }
    } catch {
      // Güvenli hata yakalama
    }
  }

  /**
   * Masabaşı Oyun Kulübü Organization JSON-LD şemasını üretir.
   */
  public buildOrganizationSchema(): Record<string, unknown> {
    return {
      '@context': 'https://schema.org',
      '@type': 'SportsClub',
      'name': 'Masabaşı Oyun Kulübü',
      'alternateName': 'Masabaşı Board Game & FRP Topluluğu',
      'url': 'https://masabasikulubu.com',
      'logo': 'https://masabasikulubu.com/images/logo.png',
      'description': "İstanbul'da her Cumartesi modern kutu oyunları ve D&D 5e masaüstü rol yapma oyunları oynayan bağımsız topluluk.",
      'address': {
        '@type': 'PostalAddress',
        'addressLocality': 'Kadıköy',
        'addressRegion': 'İstanbul',
        'addressCountry': 'TR'
      },
      'sameAs': [
        'https://www.instagram.com/masabasikulubu/',
        'https://www.meetup.com/tr-tr/masaba%c5%9f%c4%b1-oyun-kulubu/',
        'https://www.facebook.com/groups/14080585061/user/61595360127902/'
      ]
    };
  }

  /**
   * Cumartesi etkinliği için Event JSON-LD şemasını üretir.
   */
  public buildEventSchema(event: ClubEventModel): Record<string, unknown> {
    return {
      '@context': 'https://schema.org',
      '@type': 'SocialEvent',
      'name': event.title,
      'description': event.tldrSummary,
      'startDate': event.startDateUtc,
      'endDate': event.endDateUtc,
      'eventStatus': 'https://schema.org/EventScheduled',
      'eventAttendanceMode': 'https://schema.org/OfflineEventAttendanceMode',
      'location': {
        '@type': 'Place',
        'name': event.venueName,
        'address': {
          '@type': 'PostalAddress',
          'streetAddress': event.venueAddress,
          'addressLocality': 'Kadıköy',
          'addressRegion': 'İstanbul',
          'addressCountry': 'TR'
        }
      },
      'organizer': {
        '@type': 'Organization',
        'name': 'Masabaşı Oyun Kulübü',
        'url': 'https://masabasikulubu.com'
      },
      'offers': {
        '@type': 'Offer',
        'url': event.meetupEventUrl ?? 'https://masabasikulubu.com/etkinlikler',
        'price': '0',
        'priceCurrency': 'TRY',
        'availability': 'https://schema.org/InStock'
      },
      'subEvent': event.tables.map((t) => ({
        '@type': 'SocialEvent',
        'name': `Masa #${t.tableNumber}: ${t.gameTitle}`,
        'description': `Masa Yöneticisi: ${t.hostOrDmName}. Kontenjan: ${t.currentPlayers}/${t.maxPlayers}. Kategori: ${t.category}.`
      }))
    };
  }

  /**
   * Kutu oyunu ve kural sayfası için Game JSON-LD şemasını üretir.
   */
  public buildGameSchema(game: BoardGameModel): Record<string, unknown> {
    return {
      '@context': 'https://schema.org',
      '@type': 'Game',
      'name': game.title,
      'description': game.tldrSummary,
      'genre': game.category,
      'numberOfPlayers': {
        '@type': 'QuantitativeValue',
        'minValue': game.minPlayers,
        'maxValue': game.maxPlayers
      },
      'timeRequired': `PT${game.playTimeMinutes}M`,
      'inLanguage': 'tr-TR',
      'author': {
        '@type': 'Organization',
        'name': 'Masabaşı Oyun Kulübü Kütüphanesi'
      }
    };
  }

  /**
   * Blog makalesi veya kural rehberi için Article JSON-LD şemasını üretir.
   */
  public buildArticleSchema(article: ArticleModel): Record<string, unknown> {
    return {
      '@context': 'https://schema.org',
      '@type': 'Article',
      'headline': article.title,
      'description': article.metaDescription || article.tldrSummary,
      'author': {
        '@type': 'Person',
        'name': article.authorName
      },
      'publisher': {
        '@type': 'Organization',
        'name': 'Masabaşı Oyun Kulübü',
        'logo': {
          '@type': 'ImageObject',
          'url': 'https://masabasikulubu.com/images/logo.png'
        }
      },
      'datePublished': article.publishedAtUtc,
      'inLanguage': 'tr-TR',
      'mainEntityOfPage': {
        '@type': 'WebPage',
        '@id': `https://masabasikulubu.com/blog/${article.topicCluster}/${article.slug}`
      }
    };
  }

  /**
   * Sıkça Sorulan Sorular için FAQPage JSON-LD şemasını üretir.
   */
  public buildFaqSchema(faqs: Array<{ question: string; answer: string }>): Record<string, unknown> {
    return {
      '@context': 'https://schema.org',
      '@type': 'FAQPage',
      'mainEntity': faqs.map((f) => ({
        '@type': 'Question',
        'name': f.question,
        'acceptedAnswer': {
          '@type': 'Answer',
          'text': f.answer
        }
      }))
    };
  }
}
