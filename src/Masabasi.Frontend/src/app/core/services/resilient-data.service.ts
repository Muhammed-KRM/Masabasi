import { Injectable, inject, signal, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { Observable, of } from 'rxjs';
import { catchError, timeout, tap, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ClubEventModel } from '../models/event-table.model';
import { BoardGameModel } from '../models/board-game.model';
import { ArticleModel, ArticleSummaryModel } from '../models/article.model';

export type DataSourceMode = 'LIVE_API' | 'STATIC_FALLBACK';

@Injectable({ providedIn: 'root' })
export class ResilientDataService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);

  /**
   * UI bileşenlerinin şu an canlı API'den mi yoksa statik JSON snapshot'tan mı
   * beslendiğini bildiren reaktif Signal.
   */
  public readonly activeMode = signal<DataSourceMode>(
    environment.useStaticOnly ? 'STATIC_FALLBACK' : 'LIVE_API'
  );

  /**
   * Cumartesi etkinliklerini ve masa kontenjanlarını getirir.
   */
  public getEvents(): Observable<ClubEventModel[]> {
    if (environment.useStaticOnly) {
      this.activeMode.set('STATIC_FALLBACK');
      return this.http.get<ClubEventModel[]>('/data/events.snapshot.json');
    }

    return this.http
      .get<ClubEventModel[]>(`${environment.apiUrl}/api/v1/public/events`)
      .pipe(
        timeout(environment.apiTimeoutMs ?? 1500),
        tap(() => this.activeMode.set('LIVE_API')),
        catchError((err) => {
          this.logFallback('Etkinlikler', err);
          this.activeMode.set('STATIC_FALLBACK');
          return this.http.get<ClubEventModel[]>('/data/events.snapshot.json');
        })
      );
  }

  /**
   * Slug değerine göre etkinlik detayını getirir.
   */
  public getEventBySlug(slug: string): Observable<ClubEventModel | null> {
    if (environment.useStaticOnly) {
      this.activeMode.set('STATIC_FALLBACK');
      return this.getEvents().pipe(
        map((events) => events.find((e) => e.slug === slug) ?? null)
      );
    }

    return this.http
      .get<ClubEventModel>(`${environment.apiUrl}/api/v1/public/events/${slug}`)
      .pipe(
        timeout(environment.apiTimeoutMs ?? 1500),
        tap(() => this.activeMode.set('LIVE_API')),
        catchError((err) => {
          this.logFallback(`Etkinlik Detay (${slug})`, err);
          this.activeMode.set('STATIC_FALLBACK');
          return this.getEvents().pipe(
            map((events) => events.find((e) => e.slug === slug) ?? null)
          );
        })
      );
  }

  /**
   * Kutu oyunu ve FRP kütüphanesini getirir.
   */
  public getGames(): Observable<BoardGameModel[]> {
    if (environment.useStaticOnly) {
      this.activeMode.set('STATIC_FALLBACK');
      return this.http.get<BoardGameModel[]>('/data/games.snapshot.json');
    }

    return this.http
      .get<BoardGameModel[]>(`${environment.apiUrl}/api/v1/public/games`)
      .pipe(
        timeout(environment.apiTimeoutMs ?? 1500),
        tap(() => this.activeMode.set('LIVE_API')),
        catchError((err) => {
          this.logFallback('Oyun Kütüphanesi', err);
          this.activeMode.set('STATIC_FALLBACK');
          return this.http.get<BoardGameModel[]>('/data/games.snapshot.json');
        })
      );
  }

  /**
   * Slug bilgisine göre oyun detayını ve kural rehberini getirir.
   */
  public getGameBySlug(slug: string): Observable<BoardGameModel | null> {
    if (environment.useStaticOnly) {
      this.activeMode.set('STATIC_FALLBACK');
      return this.getGames().pipe(
        map((games) => games.find((g) => g.slug === slug) ?? null)
      );
    }

    return this.http
      .get<BoardGameModel>(`${environment.apiUrl}/api/v1/public/games/${slug}`)
      .pipe(
        timeout(environment.apiTimeoutMs ?? 1500),
        tap(() => this.activeMode.set('LIVE_API')),
        catchError((err) => {
          this.logFallback(`Oyun Detay (${slug})`, err);
          this.activeMode.set('STATIC_FALLBACK');
          return this.getGames().pipe(
            map((games) => games.find((g) => g.slug === slug) ?? null)
          );
        })
      );
  }

  /**
   * Blog ve SEO rehber makalelerini getirir.
   */
  public getArticles(cluster?: string): Observable<ArticleSummaryModel[]> {
    const url = cluster
      ? `${environment.apiUrl}/api/v1/public/articles?topicCluster=${cluster}`
      : `${environment.apiUrl}/api/v1/public/articles`;

    if (environment.useStaticOnly) {
      this.activeMode.set('STATIC_FALLBACK');
      return this.getStaticArticles(cluster);
    }

    return this.http
      .get<{ items: ArticleSummaryModel[] }>(url)
      .pipe(
        timeout(environment.apiTimeoutMs ?? 1500),
        map((res) => res.items ?? (res as unknown as ArticleSummaryModel[])),
        tap(() => this.activeMode.set('LIVE_API')),
        catchError((err) => {
          this.logFallback('Makaleler', err);
          this.activeMode.set('STATIC_FALLBACK');
          return this.getStaticArticles(cluster);
        })
      );
  }

  /**
   * Slug bilgisine göre makale detayını ve derlenmiş HTML içeriğini getirir.
   */
  public getArticleBySlug(slug: string): Observable<ArticleModel | null> {
    if (environment.useStaticOnly) {
      this.activeMode.set('STATIC_FALLBACK');
      return this.http.get<ArticleModel[]>('/data/articles.snapshot.json').pipe(
        map((articles) => articles.find((a) => a.slug === slug) ?? null)
      );
    }

    return this.http
      .get<ArticleModel>(`${environment.apiUrl}/api/v1/public/articles/${slug}`)
      .pipe(
        timeout(environment.apiTimeoutMs ?? 1500),
        tap(() => this.activeMode.set('LIVE_API')),
        catchError((err) => {
          this.logFallback(`Makale Detay (${slug})`, err);
          this.activeMode.set('STATIC_FALLBACK');
          return this.http.get<ArticleModel[]>('/data/articles.snapshot.json').pipe(
            map((articles) => articles.find((a) => a.slug === slug) ?? null)
          );
        })
      );
  }

  private getStaticArticles(cluster?: string): Observable<ArticleSummaryModel[]> {
    return this.http.get<ArticleModel[]>('/data/articles.snapshot.json').pipe(
      map((articles) => {
        const filtered = cluster
          ? articles.filter((a) => a.topicCluster === cluster)
          : articles;
        return filtered.map((a) => ({
          id: a.id,
          title: a.title,
          slug: a.slug,
          topicCluster: a.topicCluster,
          tldrSummary: a.tldrSummary,
          coverImageUrl: a.coverImageUrl,
          estimatedReadTimeMinutes: a.estimatedReadTimeMinutes,
          publishedAtUtc: a.publishedAtUtc,
          viewCount: a.viewCount
        }));
      })
    );
  }

  private logFallback(resource: string, err: unknown): void {
    if (isPlatformBrowser(this.platformId)) {
      console.warn(
        `[Masabaşı Resilient Mode] .NET API erişilemedi (${resource}), statik snapshot verisine geçildi:`,
        (err as Error)?.message || err
      );
    }
  }
}
