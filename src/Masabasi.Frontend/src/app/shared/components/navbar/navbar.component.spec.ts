import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { NavbarComponent } from './navbar.component';
import { ThemeService } from '../../../core/services/theme.service';

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;
  let themeService: ThemeService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NavbarComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        ThemeService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    themeService = TestBed.inject(ThemeService);
    fixture.detectChanges();
  });

  it('should render brand logo and navigation links', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.navbar__title')?.textContent).toContain('MASABAŞI');
    expect(el.textContent).toContain('Ana Sayfa');
    expect(el.textContent).toContain('Buluşmalar & Masalar');
    expect(el.textContent).toContain('Oyun Kütüphanesi');
    expect(el.textContent).toContain('FRP & D&D Rehberi');
  });

  it('should toggle mobile menu', () => {
    expect(component.isMobileMenuOpen()).toBe(false);

    component.toggleMobileMenu();
    expect(component.isMobileMenuOpen()).toBe(true);

    component.closeMobileMenu();
    expect(component.isMobileMenuOpen()).toBe(false);
  });

  it('should toggle theme on button click', () => {
    const themeBtn = fixture.nativeElement.querySelector('.navbar__theme-btn') as HTMLButtonElement;
    expect(themeService.currentTheme()).toBe('taverna');

    themeBtn.click();
    expect(themeService.currentTheme()).toBe('parchment');

    themeBtn.click();
    expect(themeService.currentTheme()).toBe('taverna');
  });
});
