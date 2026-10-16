import { Injectable, inject, signal, PLATFORM_ID } from '@angular/core';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';

export type AppTheme = 'taverna' | 'parchment';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly doc = inject(DOCUMENT);

  public readonly currentTheme = signal<AppTheme>('taverna');

  constructor() {
    if (isPlatformBrowser(this.platformId)) {
      const saved = localStorage.getItem('masabasi_theme') as AppTheme | null;
      if (saved === 'parchment' || saved === 'taverna') {
        this.setTheme(saved);
      } else {
        this.setTheme('taverna');
      }
    }
  }

  public setTheme(theme: AppTheme): void {
    this.currentTheme.set(theme);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('masabasi_theme', theme);
      if (theme === 'parchment') {
        this.doc.documentElement.setAttribute('data-theme', 'parchment');
      } else {
        this.doc.documentElement.removeAttribute('data-theme');
      }
    }
  }

  public toggleTheme(): void {
    const next = this.currentTheme() === 'taverna' ? 'parchment' : 'taverna';
    this.setTheme(next);
  }
}
