import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActiveModeBadgeComponent } from './active-mode-badge.component';
import { ResilientDataService } from '../../../core/services/resilient-data.service';

describe('ActiveModeBadgeComponent', () => {
  let component: ActiveModeBadgeComponent;
  let fixture: ComponentFixture<ActiveModeBadgeComponent>;
  let dataService: ResilientDataService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActiveModeBadgeComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ResilientDataService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ActiveModeBadgeComponent);
    component = fixture.componentInstance;
    dataService = TestBed.inject(ResilientDataService);
    fixture.detectChanges();
  });

  it('should render Live status badge when activeMode is LIVE_API', () => {
    dataService.activeMode.set('LIVE_API');
    fixture.detectChanges();

    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.mode-badge--live')).toBeTruthy();
    expect(el.textContent).toContain('Canlı Senkronizasyon');
  });

  it('should render Static status badge when activeMode is STATIC_FALLBACK', () => {
    dataService.activeMode.set('STATIC_FALLBACK');
    fixture.detectChanges();

    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.mode-badge--static')).toBeTruthy();
    expect(el.textContent).toContain('Bakırköy / İstanbul');
  });
});
