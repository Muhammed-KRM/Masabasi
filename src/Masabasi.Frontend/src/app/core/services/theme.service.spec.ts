import { TestBed } from '@angular/core/testing';
import { ThemeService } from './theme.service';

describe('ThemeService', () => {
  let service: ThemeService;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({});
    service = TestBed.inject(ThemeService);
  });

  it('should be created with default taverna theme', () => {
    expect(service).toBeTruthy();
    expect(service.currentTheme()).toBe('taverna');
  });

  it('should toggle theme between taverna and parchment', () => {
    service.toggleTheme();
    expect(service.currentTheme()).toBe('parchment');
    expect(document.documentElement.getAttribute('data-theme')).toBe('parchment');

    service.toggleTheme();
    expect(service.currentTheme()).toBe('taverna');
    expect(document.documentElement.getAttribute('data-theme')).toBeNull();
  });

  it('should set theme explicitly', () => {
    service.setTheme('parchment');
    expect(service.currentTheme()).toBe('parchment');

    service.setTheme('taverna');
    expect(service.currentTheme()).toBe('taverna');
  });
});
